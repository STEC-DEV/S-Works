using ClosedXML.Excel;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.KakaoLog;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Logging;
using System.Linq;

namespace FamTec.Server.Services.Voc
{
    public class VocService : IVocService
    {
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IAlarmInfoRepository AlarmInfoRepository;
        private readonly IFileService FileService;
        private readonly IUserInfoRepository UserInfoRepository;

        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IAdminUserInfoRepository AdminUserInfoRepository;
        private readonly IKakaoService KakaoService;

        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IHubContext<BroadcastHub> HubContext;

        private readonly IWebHostEnvironment WebHostEnvironment;

        private readonly ConsoleLogService<VocService> CreateBuilderLogger;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocFileFolderPath;
        private ILogService LogService;

        public VocService(IVocInfoRepository _vocinforepository,
            IVocCommentRepository _voccommentrepository,
            IBuildingInfoRepository _buildinginforepository,
            IAlarmInfoRepository _alarminforepository,
            IUserInfoRepository _userinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            IPlaceInfoRepository _placeinforepository,
            IBlackListInfoRepository _blacklistinforepository,
            IKakaoLogInfoRepository _kakaologinforepository,
            IHubContext<BroadcastHub> _hubcontext,
            IKakaoService _kakaoservice,
            IFileService _fileservice,
            ILogService _logservice,
            IWebHostEnvironment _webhostenvironment,
            ConsoleLogService<VocService> _createbuilderlogger)
        {
            this.VocInfoRepository = _vocinforepository;
            this.VocCommentRepository = _voccommentrepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.AlarmInfoRepository = _alarminforepository;
            this.UserInfoRepository = _userinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;

            this.PlaceInfoRepository = _placeinforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;

            this.HubContext = _hubcontext;
            this.KakaoService = _kakaoservice;

            this.WebHostEnvironment = _webhostenvironment;

            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 엑셀 데이터 IMPORT
        /// </summary>
        /// <param name="importdata"></param>
        /// <returns></returns>
        public async Task<ResponseList<ImportVocData>?> ImportVocServiceV2(HttpContext context, List<ImportVocData> importdata)
        {
            try
            {
                if (context is null)
                    return new ResponseList<ImportVocData>() { message = "인증되지 않는 사용자입니다.", data = null, code = 401 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeId))
                    return new ResponseList<ImportVocData>() { message = "인증되지 않는 사용자입니다.", data = null, code = 401 };

                // 일단 들어오면 모든 실패유무를 false로 만들고 시작한다. 
                importdata.ForEach(m => m.failYn = false); 

                // 조건검색 [1]
                // 1. 해당 사업장에 넘어온 유저 인덱스가 있는지 검사.
                // 사용자+관리자 해서 아에없으면 바로 Return
                var PlaceUserList = await AdminUserInfoRepository.GetAdminPlaceList(Convert.ToInt32(placeId));
                if (PlaceUserList is null)
                {
                    importdata.ForEach(m => m.failYn = true);
                    return new ResponseList<ImportVocData>() { message = "해당 사업장에 사용자가 존재하지 않습니다.", data = importdata, code = 401 };
                }

                // 조건검색 [1] - 2 넘어온 Excel이랑 비교해서 없는값이 있으면 해당값 false 치고 return
                List<int> dbUserIdx = PlaceUserList.Select(x => x.userIdx).ToList();

                // 2) Excel(importdata) 중 DB에 없는 userIdx를 가진 항목만 추출
                var missingItems = importdata
                    .Where(item => !dbUserIdx.Contains(item.userIdx))
                    .ToList();

                // 4) 하나라도 있으면 바로 리턴
                if (missingItems.Any())
                {
                    // 3) 해당 항목들에 failYn = true 표시
                    missingItems.ForEach(item => item.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "해당 사업장에 없는 사용자ID가 존재합니다.",
                        data = importdata,  // importdata 안의 해당 항목들이 이미 failYn=true
                        code = 204
                    };
                }

                // 조건검색 [2] 해당 사업장에 넘어온 건물 인덱스가 있는지 검사 없으면 return
                // 건물이 일단 Null이면 Rutn
                // 넘어온 Excel이랑 비교해서 없는값이 있으면 해당값 false 치고 return
                var BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeId));
                if (BuildingList is null)
                {
                    importdata.ForEach(m => m.failYn = true);
                    return new ResponseList<ImportVocData>() { message = "해당 사업장에 건물이 존재하지 않습니다.", data = null, code = 404 };
                }

                List<int>dbBuildingIdx = BuildingList.Select(x => x.Id).ToList();
                // 2) Excel(importdata) 중 DB에 없는 userIdx를 가진 항목만 추출
                var missingBuildingItem = importdata
                    .Where(item => !dbBuildingIdx.Contains(item.buildingIdx))
                    .ToList();

                // 4) 하나라도 있으면 바로 리턴
                if (missingBuildingItem.Any())
                {
                    // 3) 해당 항목들에 failYn = true 표시
                    missingBuildingItem.ForEach(item => item.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "해당 사업장에 없는 건물ID가 존재합니다.",
                        data = importdata,  // importdata 안의 해당 항목들이 이미 failYn=true
                        code = 204
                    };
                }

                // 민원인 필수값 검사
                var ItemCheck = importdata.Where(m => m.createUser is null).ToList();
                if(ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "민원인 항목은 필수값입니다.",
                        data = importdata,
                        code = 204
                    };
                }

                
                // [조건 검색 (3)]
                // 처리완료인데 처리시간이 없는 항목이 있음.
                // return 하고 fail 처리해서 알려줘야함.
                // 이 ForEach 는 원본 importdata 안의 객체를 수정합니다.
                ItemCheck = importdata.Where(m => m.status == "처리완료" && m.completeDT == null).ToList();
                if (ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "처리완료인데 처리시간이 없는 항목이 있습니다.",
                        data = importdata,   // importdata 안의 해당 항목들이 failYn=true 로 변경된 상태
                        code = 204
                    };
                }

                // 민원 발생시간 없으면 return
                ItemCheck = importdata.Where(m => m.createDT == null).ToList();
                if(ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "민원 발생일시가 없는 항목이 있습니다.",
                        data = importdata,
                        code = 204
                    };
                }

                // [조건 검색 (4)]
                // 처리완료가 아닌데 완료시간이 있는 항목이 있음.
                // return 하고 fail 처리해서 알려줘야함.
                ItemCheck = importdata.Where(m => m.status != "처리완료" && m.completeDT != null).ToList();
                if(ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);

                    return new ResponseList<ImportVocData>
                    {
                        message = "처리완료가 아닌데 완료시간이 있는 항목이 있습니다.",
                        data = importdata,
                        code = 204
                    };
                }

                // [조건 검색 (5)]
                // 민원의 제목은 필수값인데 없는 항목이 있음.
                // return 하고 fail 처리해서 알려줘야함.
                ItemCheck = importdata.Where(m => m.title is null).ToList();
                if(ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);
                    return new ResponseList<ImportVocData>
                    {
                        message = "민원의 제목은 필수입력값입니다.",
                        data = importdata,
                        code = 204
                    };
                }

                // [조건 검색 (6)]
                // 민원의 내용은 필수값인데 없는 항목이 있음.
                // return 하고 fail 처리해서 알려줘야함.
                ItemCheck = importdata.Where(m => m.contents is null).ToList();
                if(ItemCheck.Any())
                {
                    ItemCheck.ForEach(m => m.failYn = true);
                    return new ResponseList<ImportVocData>
                    {
                        message = "민원의 내용은 필수입력값입니다.",
                        data = importdata,
                        code = 204
                    };
                }

                List<ConvertVocData> ConvertList = new List<ConvertVocData>();
                foreach(var item in importdata)
                {
                    ConvertVocData model = new ConvertVocData();
                    model.title = item.title; // 민원의 제목
                    model.contents = item.contents; // 민원의 내용
                    model.createUser = item.createUser; // 민원 작성자
                    int type = 0;
                    switch(item.type)
                    {
                        case "미분류": type = 0; break;
                        case "기계": type = 1; break;
                        case "전기": type = 2; break;
                        case "승강": type = 3; break;
                        case "소방": type = 4; break;
                        case "건축": type = 5; break;
                        case "통신": type = 6; break;
                        case "미화": type = 7; break;
                        case "보안": type = 8; break;
                        default: type = 0; break;
                    }
                    model.type = type; // 민원의 종류
                    model.phone = item.phone; // 전화번호

                    int status = 0;
                    switch(item.status)
                    {
                        case "미처리": status = 0; break;
                        case "처리중": status = 1; break;
                        case "처리완료": status = 2; break;
                        default: status = 2; break;
                    }
                    model.status = status; // 민원 처리상태
                    model.createDT = item.createDT!.Value; // 민원 발생일
                    model.completeDT = item.completeDT; // 민원 완료시간
                    model.completeContents = item.completeContents; // 민원 처리내용
                    model.buildingIdx = item.buildingIdx;
                    model.userIdx = item.userIdx;
                    model.updateUser = PlaceUserList .FirstOrDefault(m => m.userIdx == item.userIdx)?.userName ?? string.Empty;

                    ConvertList.Add(model);
                }

                // 이 검증이 끝나면 --> UpdateUser 때문에 바꿔서 Repo 단으로 넘겨서
                // 한글로 된 내용 인덱스로 바꿔야되면 바꾸고
                /* 
                    미처리가 아닌 이상
                    VOC 테이블에 넣고 걔 인덱스를 Comment 에 넣어야하니
                    1건 1건 이렇게 넣고
                    Transaction 걸고
                    SaveChanged 해야할듯
                 */

                var ImportDBResult = await VocInfoRepository.ImportVocData(ConvertList).ConfigureAwait(false);
                if(ImportDBResult == 1)
                    return new ResponseList<ImportVocData>() { message = "요청이 정상처리되었습니다.", data = importdata, code = 200 };
                else if(ImportDBResult == -1)
                    return new ResponseList<ImportVocData>() { message = "접수번호 생성에 실패했습니다. 잠시후 다시 시도해주세요", data = importdata, code = 200 };
                else
                    return new ResponseList<ImportVocData>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
            catch (Exception ex) 
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<ImportVocData>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// VOC 엑셀 양식 다운로드
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]?> DownloadVocForm(HttpContext context)
        {
            try
            {
                if (context is null)
                    return null;

                string? PlaceId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceId))
                    return null;

                string? filePath = Path.Combine(WebHostEnvironment.ContentRootPath, "ExcelForm", "민원(양식).xlsx");
                if (!File.Exists(filePath))
                    return null;


                using var workbook = new XLWorkbook(filePath);
                var ws = workbook.Worksheet(1); // 건물정보 Sheet

                var BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(PlaceId));
                if (BuildingList is null || BuildingList.Count == 0)
                    return null;

                
                var UserList = await AdminUserInfoRepository.GetAdminPlaceList(Convert.ToInt32(PlaceId));
                //var UserList = await UserInfoRepository.GetPlaceUserList(Convert.ToInt32(PlaceId));
                if (UserList is null || UserList.Count == 0)
                    return null;

                for (int i = 0; i < BuildingList.Count; i++)
                {
                    ws.Cell(i + 3, 1).Value = BuildingList[i].Id;
                    IXLCell cell = ws.Cell(i + 3, 1);
                    cell.Style.Font.FontName = "맑은 고딕"; // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙 정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬

                    ws.Cell(i + 3, 2).Value = BuildingList[i].Name ?? null;
                    cell = ws.Cell(i + 3, 2);
                    cell.Style.Font.FontName = "맑은 고딕";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙 정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬
                }

             

                var ws2 = workbook.Worksheet(2); // 관리자정보 Sheet
                for (int i = 0; i < UserList.Count; i++)
                {
                    ws2.Cell(i + 3, 1).Value = UserList[i].userIdx;
                    IXLCell cell = ws2.Cell(i + 3, 1);
                    cell.Style.Font.FontName = "맑은 고딕"; // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙 정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬

                    ws2.Cell(i + 3, 2).Value = UserList[i].userName ?? null;
                    cell = ws2.Cell(i + 3, 2);
                    cell.Style.Font.FontName = "맑은 고딕";
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙 정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬
                }
                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                return ms.ToArray();

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 등록된 민원 처리내역 최신상태를 알림톡으로 전송
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> RecentVocSendService(HttpContext context, RecentVocDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "요청 권한이 없습니다.", data = false, code = 401 };
                if (dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };
                if(dto.vocId == 0)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creator = Convert.ToString(context.Items["Name"]);
                
                if (String.IsNullOrWhiteSpace(placeId) || String.IsNullOrWhiteSpace(Creator))
                    return new ResponseUnit<bool>() { message = "요청 권한이 없습니다.", data = false, code = 401 };

                // VocTB
                var VocTB = await VocInfoRepository.GetVocInfoById(dto.vocId).ConfigureAwait(false);
                if (VocTB is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };

                if (VocTB.Status == 0)
                    return new ResponseUnit<bool>() { message = "미처리 내역은 전송하실 수 없습니다.", data = false, code = 200 };
                
                // CommentTB
                var commentTB = await VocCommentRepository.GetCommentList(dto.vocId).ConfigureAwait(false);
                if(commentTB is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };

                var latestComment = commentTB.OrderByDescending(c => c.CreateDt).FirstOrDefault();
                if(latestComment is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };
                
                // PlaceTB
                var PlaceTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId).ConfigureAwait(false);
                if(PlaceTB is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 204 };

                string? receiver = VocTB.Phone; // 받는사람 전화번호
                string? placetel = PlaceTB.Tel; // 사업장 전화번호
                string url = $"https://sws.s-tec.co.kr/m/voc/select/{VocTB.Code}";

                string statusResult = String.Empty;

                if(latestComment.Status == 1)
                {
                    statusResult = "처리중";
                }
                else if(latestComment.Status == 2)
                {
                    statusResult = "처리완료";
                }

                DateTime ThisDate = DateTime.Now;

                AddKakaoLogDTO? ApiSendResult = await KakaoService.UpdateVocAnswer(VocTB.Code, statusResult, receiver!, url, placetel).ConfigureAwait(false);
                if(ApiSendResult is not null)
                {
                    // 카카오 메시지 성공 - 벤더사에 넘어가면 여기 탐
                    KakaoLogTb LogTB = new KakaoLogTb();
                    LogTB.Code = ApiSendResult.Code;
                    LogTB.Message = ApiSendResult.Message;
                    LogTB.Msgid = ApiSendResult.MSGID;
                    LogTB.Phone = ApiSendResult.Phone; // 받는사람 전화번호
                    LogTB.MsgUpdate = false; // 카카오 API 벤더에서 전송결과 유무를 받아서 전송됐는지 확인하기 위한 FLAG
                    LogTB.CreateDt = ThisDate; // 현재시간
                    LogTB.CreateUser = $"[재발송]_{Creator}"; // 작성자
                    LogTB.UpdateDt = ThisDate; // 현재시간
                    LogTB.UpdateUser = Creator; // 작성자
                    LogTB.VocTbId = VocTB.Id; // 민원 ID
                    LogTB.PlaceTbId = PlaceTB.Id; // 사업장 ID
                    LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물 ID

                    var LogTBResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                    if (LogTBResult is null)
                        return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
                else
                {
                    // 카카오 메시지 실패
                    KakaoLogTb LogTB = new KakaoLogTb();
                    LogTB.Code = null;
                    LogTB.Message = "500";
                    LogTB.Msgid = null;
                    LogTB.Phone = receiver; // 받는사람 전화번호
                    LogTB.MsgUpdate = true;
                    LogTB.CreateDt = ThisDate; // 현재시간
                    LogTB.CreateUser = Creator; // 작성자
                    LogTB.UpdateDt = ThisDate; // 현재시간
                    LogTB.UpdateUser = Creator; // 작성자
                    LogTB.VocTbId = VocTB.Id; // 민원 ID
                    LogTB.PlaceTbId = PlaceTB.Id; // 사업장 ID
                    LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물 ID

                    var LogTBResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                    if(LogTBResult is null)
                        return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
                return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }


        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<AllVocListDTO>> GetVocList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AllVocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<AllVocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<AllVocListDTO>? model = await VocInfoRepository.GetVocList(Convert.ToInt32(placeid), type, status, buildingid, division).ConfigureAwait(false);
                if (model is [_, ..])
                    return new ResponseList<AllVocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<AllVocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<AllVocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장의 선택된 일자의 VOC LIST 반환
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocListDTO>> GetVocFilterList(HttpContext context, DateTime startdate, DateTime enddate, List<int> type, List<int> status,List<int> buildingid, List<int>division)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };


                List<VocListDTO>? model = await VocInfoRepository.GetVocFilterList(Convert.ToInt32(PlaceIdx), startdate, enddate, type, status, buildingid, division).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTO>(), code = 500 };
            }
        }



        /// <summary>
        /// 월간 사업장별 VOC 조회 - V2
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocListDTOV2>> GetMonthVocSearchListV2(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (String.IsNullOrWhiteSpace(searchDate))
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string[] splitDate = searchDate.Split('-');
                string year = splitDate[0];
                string month = splitDate[1].PadLeft(2, '0'); // 한 자리 월을 두 자리로 맞추기 위해 앞에 0 추가

                int checkResult;

                // 년도 숫자값 맞는지 검사
                bool checkDate = int.TryParse(year, out checkResult);
                if (!checkDate)
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 월 숫자값 맞는지 검사
                checkDate = int.TryParse(month, out checkResult);
                if (!checkDate)
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<VocListDTOV2>? model = await VocInfoRepository.GetVocMonthListV2(Convert.ToInt32(placeid), type, status, buildingid, division, Convert.ToInt32(year), Convert.ToInt32(month)).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseList<VocListDTOV2>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTOV2>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocListDTOV2>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTOV2>(), code = 500 };
            }
        }

        /// <summary>
        /// 월간 사업장 VOC 조회 [Regacy]
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocListDTO>> GetMonthVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                  if (String.IsNullOrWhiteSpace(searchDate))
                      return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                  string[] splitDate = searchDate.Split('-');
                  string year = splitDate[0];
                  string month = splitDate[1].PadLeft(2, '0'); // 한 자리 월을 두 자리로 맞추기 위해 앞에 0 추가

                int checkResult;

                // 년도 숫자값 맞는지 검사
                bool checkDate = int.TryParse(year, out checkResult);
                if (!checkDate)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 월 숫자값 맞는지 검사
                checkDate = int.TryParse(month, out checkResult);
                if (!checkDate)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<VocListDTO>? model = await VocInfoRepository.GetVocMonthList(Convert.ToInt32(placeid), type, status, buildingid, division, Convert.ToInt32(year), Convert.ToInt32(month)).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTO>(), code = 500 };
            }
        }

     

        /// <summary>
        /// 기간 사업장 VOC 조회 -V2
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocListDTOV2>> GetDateVocSearchListV2(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = new List<VocListDTOV2>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocListDTOV2>() { message = "잘못된 요청입니다.", data = new List<VocListDTOV2>(), code = 404 };

                List<VocListDTOV2>? model = await VocInfoRepository.GetVocFilterListV2(Convert.ToInt32(PlaceIdx), StartDate, EndDate, type, status, buildingid, division).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseList<VocListDTOV2>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTOV2>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocListDTOV2>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTOV2>(), code = 500 };
            }
        }

        /// <summary>
        /// 기간 사업장 VOC 조회 [Regacy]
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocListDTO>> GetDateVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };

                List<VocListDTO>? model = await VocInfoRepository.GetVocFilterList(Convert.ToInt32(PlaceIdx), StartDate, EndDate, type, status, buildingid, division).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTO>(), code = 500 };
            }
        }



        /// <summary>
        /// VOC 상세보기 - 직원용
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<VocEmployeeDetailDTO>> GetVocDetail(HttpContext context, int vocid, bool isMobile)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<VocEmployeeDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseUnit<VocEmployeeDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? model = await VocInfoRepository.GetVocInfoById(vocid).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<VocEmployeeDetailDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                BuildingTb? building = await BuildingInfoRepository.GetBuildingInfo(model.BuildingTbId).ConfigureAwait(false);
                if(building is null)
                    return new ResponseUnit<VocEmployeeDetailDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                VocEmployeeDetailDTO dto = new VocEmployeeDetailDTO();
                dto.Id = model.Id; // 민원 인덱스
                dto.Code = model.Code; // 접수코드
                dto.CreateDT = model.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 민원 신청일
                dto.Status = model.Status; // 민원상태
                dto.BuildingName = building.Name; // 건물명
                dto.Type = model.Type;
                dto.Division = model.Division; // 웹 - 모바일 구분
                dto.Title = model.Title; // 민원제목
                dto.Contents = model.Content; // 민원내용
                dto.CreateUser = model.CreateUser; // 민원인
                dto.Phone = model.Phone; // 민원인 전화번호

                string VocFileName = Path.Combine(Common.FileServer, PlaceIdx.ToString(), "Voc", model.Id.ToString());
                di = new DirectoryInfo(VocFileName);
                if (!di.Exists) di.Create();

                if(isMobile)
                {
                    // 모바일
                    var ImageFiles = new[] { model.Image1, model.Image2, model.Image3 };

                    foreach (var image in ImageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, image).ConfigureAwait(false);

                            if(ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if(files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                    if (ConvertFile is not null)
                                    {
                                        dto.ImageName.Add(image);
                                        dto.Images.Add(ConvertFile);
                                    }
                                    else
                                    {
                                        dto.ImageName.Add(null);
                                        dto.Images.Add(null);
                                    }
                                }
                                else
                                {
                                    dto.ImageName.Add(null);
                                    dto.Images.Add(null);
                                }
                            }
                            else
                            {
                                dto.ImageName.Add(null);
                                dto.Images.Add(null);
                            }
                        }
                        else // 이미지 명칭에 DB에 없음
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }
                }
                else
                {
                    // PC
                    var imageFiles = new[] { model.Image1, model.Image2, model.Image3 };
                    foreach (var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image)) // 이미지명칭이 DB에 있으면
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, image).ConfigureAwait(false);

                            if (ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if (files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

                                    if (ConvertFile is not null)
                                    {
                                        dto.ImageName.Add(image);
                                        dto.Images.Add(ConvertFile);
                                    }
                                    else
                                    {
                                        dto.ImageName.Add(null);
                                        dto.Images.Add(null);
                                    }
                                }
                                else
                                {
                                    dto.ImageName.Add(null);
                                    dto.Images.Add(null);
                                }
                            }
                            else 
                            {
                                dto.ImageName.Add(null);
                                dto.Images.Add(null);
                            }
                        } // 이미지 명칭에 DB에 없으면.
                        else
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }
                }


                return new ResponseUnit<VocEmployeeDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<VocEmployeeDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// voc 유형 변경 -- 여기 바꿔야함
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateVocTypeService(HttpContext context, UpdateVocDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID!.Value).ConfigureAwait(false);
                if(VocTB is null)
                    return new ResponseUnit<bool?>() { message = "조회결과가 존재하지 않습니다.", data = null, code = 404 };

                VocTB.Type = dto.Type!.Value;
                VocTB.UpdateDt = ThisDate;
                VocTB.UpdateUser = creater;

                bool UpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB).ConfigureAwait(false);
                if(!UpdateResult)
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

                
                BuildingTb? BuildingTB;
                List<UsersTb>? Users;
                switch (dto.Type)
                {
#region 기타민원 타입변경
                    case 0:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if(VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocDefaultList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if (Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 0).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 기계민원 타입변경
                    case 1:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if(VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if (BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocMachineList(BuildingTB!.PlaceTbId).ConfigureAwait(false);

                        if (Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 1).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_MCRoom").SendAsync("ReceiveVoc", "[기계] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }
                                
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 전기민원 타입변경
                    case 2:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocElecList(BuildingTB!.PlaceTbId).ConfigureAwait(false);

                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 2).ConfigureAwait(false);


                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_ELECRoom").SendAsync("ReceiveVoc", "[전기] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion
#region 승강민원 타입변경
                    case 3:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocLiftList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 3).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_LFRoom").SendAsync("ReceiveVoc", "[승강] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 소방민원 타입변경
                    case 4:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocFireList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 4).ConfigureAwait(false);


                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_FRRoom").SendAsync("ReceiveVoc", "[소방] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 건축민원 타입변경
                    case 5:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if (BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocConstructList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 5).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_CSTRoom").SendAsync("ReceiveVoc", "[건축] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }
                                
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 통신민원 타입변경
                    // 통신
                    case 6:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocNetWorkList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 6).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_NTRoom").SendAsync("ReceiveVoc", "[통신] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 미화민원 타입변경
                    // 미화
                    case 7:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if(BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocBeautyList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 7).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_BEAUTYRoom").SendAsync("ReceiveVoc", "[미화] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }
                                
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 보안민원 타입변경
                    // 보안
                    case 8:
                        VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID.Value).ConfigureAwait(false);
                        if (VocTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId).ConfigureAwait(false);
                        if (BuildingTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                        Users = await UserInfoRepository.GetVocSecurityList(BuildingTB!.PlaceTbId).ConfigureAwait(false);
                        if(Users is [_, ..])
                        {
                            // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                            //await SetMessage(Users, VocTB.CreateUser!, dto.VocID.Value);
                            await AlarmInfoRepository.AddAlarmList(Users, creater, 1, dto.VocID.Value, 8).ConfigureAwait(false);

                            // 소켓전송
                            await HubContext.Clients.Group($"{placeidx}_SECURoom").SendAsync("ReceiveVoc", "[보안] 민원 등록되었습니다").ConfigureAwait(false);
                            await HubContext.Clients.Group($"{placeidx}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion
                }

                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
             

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// VOC 알람메시지 처리
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetMessage(List<UsersTb>? userlist, string Creater, int VocTableIdx)
        {
            try
            {
                if(userlist is null || !userlist.Any())
                    return true;

                DateTime ThisDate = DateTime.Now;

                foreach (var user in userlist)
                {
                    // 사용자 수만큼 Alarm 테이블에 Insert
                    AlarmTb alarm = new AlarmTb()
                    {
                        CreateDt = ThisDate,
                        CreateUser = Creater,
                        UpdateDt = ThisDate,
                        UpdateUser = Creater,
                        UsersTbId = user.Id,
                        VocTbId = VocTableIdx
                    };

                    AlarmTb? alarm_result = await AlarmInfoRepository.AddAsync(alarm).ConfigureAwait(false);
                    if (alarm_result is null)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// DashBoard용 일주일치 민원 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocWeekCountDTO>?> GetVocDashBoardWeeksDataService(HttpContext context)
        {
            try
            {
                #region Regacy
                /*
                   if (context is null)
                       return new ResponseUnit<VocWeekCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                   string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                   if(String.IsNullOrWhiteSpace(placeidx))
                       return new ResponseUnit<VocWeekCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                   DateTime NowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                   // 현재 요일 (0: 일요일, 1: 월요일, ..., 6: 토요일)
                   DayOfWeek currentDayOfWeek = NowDate.DayOfWeek;


                   // 현재 날짜가 있는 주의 첫날(월요일)을 구하기 위해 현재 요일에서 DayOfWeek.Monday를 빼기
                   int daysToSubtract = (int)currentDayOfWeek - (int)DayOfWeek.Monday;

                   // 일요일인 경우, 주의 첫날을 월요일로 설정하기 위해 7을 더함
                   if (daysToSubtract < 0)
                   {
                       daysToSubtract += 7;
                   }

                   // 주의 첫날(월요일) 계산
                   DateTime startOfWeek = NowDate.AddDays(-daysToSubtract);
                   DateTime EndOfWeek = startOfWeek.AddDays(7);
            */
                #endregion

                if (context is null)
                    return new ResponseList<VocWeekCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<VocWeekCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime startOfWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
              
                DateTime EndOfWeek = startOfWeek.AddDays(-7);


                List<VocWeekCountDTO>? model = await VocInfoRepository.GetDashBoardWeeksData(startOfWeek, EndOfWeek, Convert.ToInt32(placeidx)).ConfigureAwait(false);


                if (model is not null && model.Any())
                {
                    return new ResponseList<VocWeekCountDTO>() { message = "요청이 정상처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<VocWeekCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocWeekCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 대쉬보드용 금일 유형별 건수 (기타, 기계, 건설, 미화 ..)
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<VocDaysCountDTO>?> GetVocDashBoardDaysDataService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<VocDaysCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<VocDaysCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime NowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                VocDaysCountDTO? model = await VocInfoRepository.GetDashBoardDaysData(NowDate, Convert.ToInt32(placeidx)).ConfigureAwait(false);
                if(model is not null)
                {
                    
                    return new ResponseUnit<VocDaysCountDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseUnit<VocDaysCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<VocDaysCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// DashBoard용 금일 처리유형별 발생건수 (미처리, 처리중, 처리완료)
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<VocDaysStatusCountDTO>?> GetVocDaysStatusDataService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<VocDaysStatusCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<VocDaysStatusCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime NowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                VocDaysStatusCountDTO? model = await VocInfoRepository.GetDashBoardDaysStatusData(NowDate,Convert.ToInt32(placeidx)).ConfigureAwait(false);

                if(model is not null)
                {
                    
                    return new ResponseUnit<VocDaysStatusCountDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseUnit<VocDaysStatusCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<VocDaysStatusCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// DashBoard용 일주일치 처리유형별 발생건수 (미처리, 처리중, 처리완료)
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<VocWeekStatusCountDTO>?> GetVocWeeksStatusDataService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocWeekStatusCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<VocWeekStatusCountDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 함수시작일 금일 -7일
                DateTime ToDays = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                DateTime StartDate = ToDays.AddDays(-7);
                DateTime EndOfWeek = ToDays.AddDays(1).AddTicks(-1);

                List<VocWeekStatusCountDTO>? model = await VocInfoRepository.GetDashBoardWeeksStatusData(StartDate, EndOfWeek, Convert.ToInt32(placeidx));

                /*
                var result = model?
                .SelectMany(v => new[]
                {
                    new { Date = v.Date, Count = v.UnProcessed, Status = "미처리" },
                    new { Date = v.Date, Count = v.Processing, Status = "처리중" },
                    new { Date = v.Date, Count = v.Completed, Status = "처리완료" }
                })
                .GroupBy(x => new { x.Date, x.Status })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    Count = g.Sum(x => x.Count),
                    Status = g.Key.Status
                })
                .ToList();

                var result2 = result.GroupBy(m => m.Status);
                */

                

                if (model is not null)
                {
                    return new ResponseList<VocWeekStatusCountDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<VocWeekStatusCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocWeekStatusCountDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

  
    }
}
