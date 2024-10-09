using DocumentFormat.OpenXml.Wordprocessing;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace FamTec.Server.Services.Voc.Hub
{
    public class HubService : IHubService
    {
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly IAlarmInfoRepository AlarmInfoRepository;

        IHubContext<BroadcastHub> HubContext;
        private readonly IKakaoService KakaoService;
        private readonly IFileService FileService;
        private ILogService LogService;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocFileFolderPath;
        private string? VocCommentFileFolderPath;

        public HubService(IVocInfoRepository _vocinforepository,
            IVocCommentRepository _voccommentrepository,
            IPlaceInfoRepository _placeinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IBlackListInfoRepository _blacklistinforepository,
            IUserInfoRepository _userinforepository,
            IHubContext<BroadcastHub> _hubcontext,
            IKakaoLogInfoRepository _kakaologinforepository,
            IAlarmInfoRepository _alarminforepository,
            IKakaoService _kakaoservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.VocInfoRepository = _vocinforepository;
            this.VocCommentRepository = _voccommentrepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.UserInfoRepository = _userinforepository;
            this.AlarmInfoRepository = _alarminforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.KakaoService = _kakaoservice;
            this.HubContext = _hubcontext;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원 추가 - 민원인 전용
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddVocReturnDTO?>> AddVocService(AddVocDTO dto, List<IFormFile>? files)
        {
            try
            {
                List<string> newFileName = new List<string>();

                if (files is not null)
                {
                    foreach (IFormFile file in files)
                    {
                        newFileName.Add(FileService.SetNewFileName(dto.Name!, file));
                    }
                }

                if (dto is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (String.IsNullOrWhiteSpace(dto.Title) || String.IsNullOrWhiteSpace(dto.Contents) || String.IsNullOrWhiteSpace(dto.Name))
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(!String.IsNullOrWhiteSpace(dto.PhoneNumber))
                {
                    int? PhoneNumber = Convert.ToInt32(dto.PhoneNumber);
                    if(PhoneNumber is null)
                        return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                DateTime ThisDate = DateTime.Now;

                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(dto.Buildingid!.Value).ConfigureAwait(false);
                if (buildingtb is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "존재하지 않는 건물입니다.", data = null, code = 404 };

                VocTb? model = new VocTb();
                model.Title = dto.Title;
                model.Content = dto.Contents;
                model.Type = 0; // 처음은 미분류
                model.Phone = dto.PhoneNumber; // 전화번호
                model.Status = 0; // 미처리

                if (dto.PhoneNumber is null) // 전화번호 있으면 회신True / 없으면False
                    model.ReplyYn = false;
                else
                    model.ReplyYn = true;
                model.CreateDt = ThisDate;
                model.CreateUser = dto.Name; // 작성자
                model.UpdateDt = ThisDate;
                model.UpdateUser = dto.Name; // 작성자
                model.BuildingTbId = dto.Buildingid.Value;

                // 조회코드
                string? ReceiptCode;

                // 접수번호 동일한거 안나올때 까지 do-while
                do
                {
                    // VOC 에서 SELECT
                    ReceiptCode = KakaoService.RandomCode();
                    model.Code = ReceiptCode;
                } while (await VocInfoRepository.GetVocInfoByCode(ReceiptCode).ConfigureAwait(false) != null);


                // 파일이 있으면
                if (files is not null && files.Count > 0)
                {
                    // 이미지 저장
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (i is 0) model.Image1 = newFileName[i];
                        if (i is 1) model.Image2 = newFileName[i];
                        if (i is 2) model.Image3 = newFileName[i];
                    }
                }

                // 여기서 블랙리스트 조회 
                BlacklistTb? BlackListTB = await BlackListInfoRepository.GetBlackListInfo(model.Phone!).ConfigureAwait(false);
                if (BlackListTB is not null) // 블랙리스트임.
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? result = await VocInfoRepository.AddAsync(model).ConfigureAwait(false);
                if (result is not null)
                {
                    // VOC관련한 폴더 없으면 만들기 - bin/fileserevice/3/Voc/1
                    VocFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}", Common.FileServer, dto.Placeid, result.Id);
                    di = new DirectoryInfo(VocFileFolderPath);
                    if (!di.Exists) di.Create();

                    if (files is not null) // 파일 넣기
                    {
                        for (int i = 0; i < files.Count; i++) 
                        {
                            await FileService.AddResizeImageFile(newFileName[i], VocFileFolderPath, files[i]).ConfigureAwait(false);
                        }
                    }

                    // 소켓알림! + 카카오 API 알림
                    if (result.ReplyYn == true)
                    {
                        PlaceTb? placeTB = await PlaceInfoRepository.GetBuildingPlace(result.BuildingTbId).ConfigureAwait(false);

                        if (placeTB is null)
                            return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                        // 제목
                        string Title = model.Title.Length > 8 ? model.Title.Substring(0, 8) + "..." : model.Title;

                        DateTime DateNow = DateTime.Now;

                        // 전화번호
                        string receiver = model.Phone!;

                        // 사업장 전화번호
                        string placetel = placeTB.Tel!;

                        // URL 파라미터 인코딩
                        byte[] bytes = Encoding.Unicode.GetBytes(result.Id.ToString());
                        string base64 = Convert.ToBase64String(bytes);

                        /* 테스트 */
                        string url = $"http://125.131.105.172:5245/vocinfo/{base64}";
                        //string url = $"https://sws.s-tec.co.kr/vocinfo?vocid={base64}";

                        // 카카오 API 전송
                        // 보낸 USER 휴대폰번호에 전송.
                        AddKakaoLogDTO? LogDTO = await KakaoService.AddVocAnswer(Title, model.Code, DateNow, receiver, url, placetel).ConfigureAwait(false);
                        if (LogDTO is not null)
                        {
                            // 카카오 메시지 성공
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = LogDTO.Code!;
                            LogTB.Message = LogDTO.Message!;
                            LogTB.CreateDt = ThisDate;
                            LogTB.CreateUser = model.CreateUser;
                            LogTB.UpdateDt = ThisDate;
                            LogTB.UpdateUser = model.UpdateUser;
                            LogTB.VocTbId = model.Id; // 민원ID
                            LogTB.PlaceTbId = dto.Placeid!.Value; // 사업장ID
                            LogTB.BuildingTbId = dto.Buildingid!.Value; // 건물ID

                            // 카카오API 로그 테이블에 쌓아야함.
                            await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                        }
                        else
                        {
                            // 카카오 메시지 에러
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = "ERROR";
                            LogTB.Message = "ERROR";
                            LogTB.CreateDt = ThisDate;
                            LogTB.CreateUser = model.CreateUser;
                            LogTB.UpdateDt = ThisDate;
                            LogTB.UpdateUser = model.UpdateUser;
                            LogTB.VocTbId = model.Id; // 민원ID
                            LogTB.PlaceTbId = dto.Placeid!.Value; // 사업장ID
                            LogTB.BuildingTbId = dto.Buildingid!.Value; // 건물ID

                            // 카카오API 로그 테이블에 쌓아야함.
                            await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                        }
                    }

                    List<UsersTb>? UserList = await UserInfoRepository.GetVocDefaultList(dto.Placeid!.Value).ConfigureAwait(false);
                    if(UserList is [_, ..])
                    {
                        await AlarmInfoRepository.AddAlarmList(UserList, dto.Name, 0, result.Id).ConfigureAwait(false);
                    }
                   
                    // 이부분은 Voc Count를 변경할만한 곳에 넣어야함. -- 민원이 등록되는 HubController에 넣어야함.
                    await HubContext.Clients.Group($"{dto.Placeid}_VocCount").SendAsync("ReceiveVocCount", $"이 요청을 받으면 프론트에서 api/Voc/sign/GetVocWeekCount 를 Get으로 요청하도록 만들어야함.").ConfigureAwait(false);
                    await HubContext.Clients.Group($"{dto.Placeid}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다").ConfigureAwait(false);
                    
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "요청이 정상 처리되었습니다.", data = new AddVocReturnDTO
                    {
                        ReceiptCode = ReceiptCode, // 접수번호
                        PhoneNumber = result.Phone, // 전화번호
                        CreateDT = result.CreateDt.ToString("yyyy-MM-dd HH:mm:ss") // 생성일
                    }, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddVocReturnDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원 조회 - 민원인 전용
        /// </summary>
        /// <param name="voccode"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<VocUserDetailDTO?>> GetVocRecord(string? voccode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(voccode))
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocModel = await VocInfoRepository.GetVocInfoByCode(voccode).ConfigureAwait(false);
                if (VocModel is null)
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                BuildingTb? BuildingModel = await BuildingInfoRepository.GetBuildingInfo(VocModel.BuildingTbId).ConfigureAwait(false);
                if (BuildingModel is null)
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                VocUserDetailDTO dto = new VocUserDetailDTO();
                dto.Id = VocModel.Id; // 민원 인덱스
                dto.Code = VocModel.Code; // 접수 코드
                dto.CreateDT = VocModel.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 접수일
                dto.Status = VocModel.Status; // 민원 상태
                dto.BuildingName = BuildingModel.Name; // 건물명
                dto.Type = VocModel.Type switch
                {
                    0 => "미분류",
                    1 => "기계민원",
                    2 => "전기민원",
                    3 => "승강민원",
                    4 => "소방민원",
                    5 => "건축민원",
                    6 => "통신민원",
                    7 => "미화민원",
                    8 => "보안민원",
                    _ => "미분류"
                };
                dto.Title = VocModel.Title; // 민원 제목
                dto.Contents = VocModel.Content; // 민원 제목
                dto.CreateUser = VocModel.CreateUser; // 민원 신청자 이름
                dto.Phone = VocModel.Phone; // 민원인 전화번호


                VocFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}", Common.FileServer, BuildingModel.PlaceTbId, VocModel.Id);
                di = new DirectoryInfo(VocFileFolderPath);
                if (!di.Exists) di.Create();

                var imageFiles = new[] { VocModel.Image1, VocModel.Image2, VocModel.Image3 };
                foreach (var image in imageFiles)
                {
                    if (!String.IsNullOrWhiteSpace(image)) // 이미지명칭이 DB에 있으면
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(VocFileFolderPath, image).ConfigureAwait(false);
                        if (ImageBytes is not null)
                        {
                            dto.ImageName.Add(image);
                            dto.Images.Add(ImageBytes);
                        }
                        else
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }
                    else // 이미지 명칭에 DB에 없으면.
                    {
                        dto.ImageName.Add(null);
                        dto.Images.Add(null);
                    }
                }
                return new ResponseUnit<VocUserDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocUserDetailDTO?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원 댓글조회 - 민원인 전용
        /// </summary>
        /// <param name="voccode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseList<VocCommentListDTO>?> GetVocCommentList(string? voccode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(voccode))
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoByCode(voccode).ConfigureAwait(false);
                if(VocTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 404 };

                List<CommentTb>? model = await VocCommentRepository.GetCommentList(VocTB.Id).ConfigureAwait(false);
                if(model is null || model.Count == 0)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<VocCommentListDTO>(), code = 200 };

                BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB.BuildingTbId).ConfigureAwait(false);
                if(BuildingTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, BuildingTB!.PlaceTbId, VocTB.Id);
                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                List<VocCommentListDTO> dto = new List<VocCommentListDTO>();
                for (int i = 0; i < model.Count; i++)
                {
                    VocCommentListDTO dtoModel = new VocCommentListDTO();
                    dtoModel.Id = model[i].Id;
                    dtoModel.status = model[i].Status; // 댓글상태
                    dtoModel.Comment = model[i].Content; // 내용
                    dtoModel.CreateDT = model[i].CreateDt.ToString("yyyy-MM-dd HH:mm:ss");
                    dtoModel.CreateUser = model[i].CreateUser;
                    dtoModel.VocTbId = model[i].VocTbId;

                    var imageFiles = new[] { model[i].Image1, model[i].Image2, model[i].Image3 };
                    foreach (var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image)) // 이미지명칭이 DB에 있으면
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);
                            if (ImageBytes is not null)
                            {
                                dtoModel.ImageName.Add(image);
                                dtoModel.Images.Add(ImageBytes);
                            }
                            else
                            {
                                dtoModel.ImageName.Add(null);
                                dtoModel.Images.Add(null);
                            }
                        }
                        else // 이미지 명칭에 DB에 없으면.
                        {
                            dtoModel.ImageName.Add(null);
                            dtoModel.Images.Add(null);
                        }
                    }

                    dto.Add(dtoModel);
                }

                return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocCommentListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원댓글 상세보기 - 민원인 전용
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(int? commentid)
        {
            try
            {
                if (commentid is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                CommentTb? model = await VocCommentRepository.GetCommentInfo(commentid.Value).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId).ConfigureAwait(false);
                if(VocTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                PlaceTb? PlaceTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId).ConfigureAwait(false);
                if (PlaceTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentDetailDTO dto = new VocCommentDetailDTO();
                dto.VocCommentId = model.Id; // VOC 댓글 ID
                dto.Content = model.Content; // VOC 댓글 내용
                dto.Status = model.Status; // VOC 댓글 상태
                dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                dto.CreateUser = model.CreateUser; // 댓글 작성자

                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, PlaceTB.Id, VocTB.Id);
                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                var imageFiles = new[] { model.Image1, model.Image2, model.Image3 };
                foreach (var image in imageFiles)
                {
                    if (!String.IsNullOrWhiteSpace(image)) // 이미지명칭이 DB에 있으면
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);
                        if (ImageBytes is not null)
                        {
                            dto.ImageName.Add(image);
                            dto.Images.Add(ImageBytes);
                        }
                        else
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }
                    else // 이미지 명칭에 DB에 없으면.
                    {
                        dto.ImageName.Add(null);
                        dto.Images.Add(null);
                    }
                }

                return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocCommentDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
