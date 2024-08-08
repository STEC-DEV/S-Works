using FamTec.Client.Pages.Admin.Place.PlaceMain;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
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
            IKakaoLogInfoRepository _kakaologinforepository,
            IKakaoService _kakaoservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.VocInfoRepository = _vocinforepository;
            this.VocCommentRepository = _voccommentrepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.KakaoService = _kakaoservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원 추가 - 민원인 전용
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddVocReturnDTO?>> AddVocService(AddVocDTO? dto, List<IFormFile>? files)
        {
            try
            {
                if (dto is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (String.IsNullOrWhiteSpace(dto.Title))
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (String.IsNullOrWhiteSpace(dto.Contents))
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(dto.Buildingid);
                if (buildingtb is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // VOC관련한 폴더 없으면 만들기
                VocFileFolderPath = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, dto.Placeid);

                di = new DirectoryInfo(VocFileFolderPath);
                if (!di.Exists) di.Create();

                VocTb? model = new VocTb();
                model.Title = dto.Title;
                model.Content = dto.Contents;
                model.Type = 0; // 처음은 미분류
                model.Phone = dto.PhoneNumber; // 전화번호;
                model.Status = 0; // 미처리

                if (dto.PhoneNumber is null) // 전화번호 있으면 회신True / 없으면False
                    model.ReplyYn = false;
                else
                    model.ReplyYn = true;
                model.CreateDt = DateTime.Now;
                model.CreateUser = dto.Name; // 작성자
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = dto.Name; // 작성자
                model.BuildingTbId = dto.Buildingid;

                // 조회코드
                string? ReceiptCode;
                while (true)
                {
                    ReceiptCode = KakaoService.RandomCode();

                    // VOC 에서 SELECT
                    VocTb? CodeCheck = await VocInfoRepository.GetVocInfoByCode(ReceiptCode);
                    if (CodeCheck is null)
                    {
                        model.Code = ReceiptCode;
                        break;
                    }
                }

                // 파일이 있으면
                if (files is [_, ..])
                {
                    // 이미지 저장
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (i is 0)
                            model.Image1 = await FileService.AddImageFile(VocFileFolderPath, files[i]);
                        if (i is 1)
                            model.Image2 = await FileService.AddImageFile(VocFileFolderPath, files[i]);
                        if (i is 2)
                            model.Image3 = await FileService.AddImageFile(VocFileFolderPath, files[i]);
                    }
                }

                VocTb? result = await VocInfoRepository.AddAsync(model);
                if (result is not null)
                {
                    // 소켓알림! + 카카오 API 알림
                    if (result.ReplyYn == true)
                    {
                        // 여기서 블랙리스트 조회 
                        BlacklistTb? BlackListTB = await BlackListInfoRepository.GetBlackListInfo(model.Phone);

                        if (BlackListTB is null) // 블랙리스트가 아닌사람
                        {
                            PlaceTb? placeTB = await PlaceInfoRepository.GetBuildingPlace(result.BuildingTbId);

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

                            string url = $"https://123.2.156.148/vocinfo?vocid={base64}";

                            // 카카오 API 전송
                            // 보낸 USER 휴대폰번호에 전송.
                            bool? SendResult = await KakaoService.AddVocAnswer(Title, model.Code, DateNow, receiver, url, placetel);
                            if (SendResult == true)
                            {
                                // 카카오 메시지 성공
                                KakaoLogTb LogTB = new KakaoLogTb();
                                //LogTB.Result = "Y";
                                //LogTB.Title = Title;
                                LogTB.CreateDt = DateTime.Now;
                                LogTB.CreateUser = model.CreateUser;
                                LogTB.UpdateDt = DateTime.Now;
                                LogTB.UpdateUser = model.UpdateUser;
                                LogTB.VocTbId = model.Id; // 민원ID
                                LogTB.PlaceTbId = dto.Placeid; // 사업장ID
                                LogTB.BuildingTbId = dto.Buildingid; // 건물ID

                                // 카카오API 로그 테이블에 쌓아야함.
                                KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB);
                            }
                            else if (SendResult == false)
                            {
                                // 카카오 메시지 실패
                                KakaoLogTb LogTB = new KakaoLogTb();
                                //LogTB.Result = "N";
                                //LogTB.Title = Title;
                                LogTB.CreateDt = DateTime.Now;
                                LogTB.CreateUser = model.CreateUser;
                                LogTB.UpdateDt = DateTime.Now;
                                LogTB.UpdateUser = model.UpdateUser;
                                LogTB.VocTbId = model.Id; // 민원ID
                                LogTB.PlaceTbId = dto.Placeid; // 사업장ID
                                LogTB.BuildingTbId = dto.Buildingid; // 건물ID

                                // 카카오API 로그 테이블에 쌓아야함.
                                KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB);
                            }
                            else
                            {
                                // 카카오 메시지 에러
                                KakaoLogTb LogTB = new KakaoLogTb();
                                //LogTB.Result = "E";
                                //LogTB.Title = Title;
                                LogTB.CreateDt = DateTime.Now;
                                LogTB.CreateUser = model.CreateUser;
                                LogTB.UpdateDt = DateTime.Now;
                                LogTB.UpdateUser = model.UpdateUser;
                                LogTB.VocTbId = model.Id; // 민원ID
                                LogTB.PlaceTbId = dto.Placeid; // 사업장ID
                                LogTB.BuildingTbId = dto.Buildingid; // 건물ID

                                // 카카오API 로그 테이블에 쌓아야함.
                                KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB);
                            }
                        }
                    }

                    await HubContext.Clients.Group($"{dto.Placeid}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다");
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "요청이 정상 처리되었습니다.", data = new AddVocReturnDTO
                    {
                        ReceiptCode = ReceiptCode, // 접수번호
                        PhoneNumber = result.Phone, // 전화번호
                        CreateDT = result.CreateDt // 생성일
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
        public async ValueTask<ResponseUnit<VocUserDetailDTO?>> GetVocRecord(string? voccode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(voccode))
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocModel = await VocInfoRepository.GetVocInfoByCode(voccode);
                if (VocModel is null)
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                BuildingTb? BuildingModel = await BuildingInfoRepository.GetBuildingInfo(VocModel.BuildingTbId);
                if (BuildingModel is null)
                    return new ResponseUnit<VocUserDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                VocUserDetailDTO dto = new VocUserDetailDTO();
                dto.Id = VocModel.Id; // 민원 인덱스
                dto.Code = VocModel.Code; // 접수 코드
                dto.CreateDT = VocModel.CreateDt; // 접수일
                dto.Status = VocModel.Status; // 민원 상태
                dto.BuildingName = BuildingModel.Name; // 건물명
                dto.Type = VocModel.Type; // 민원 유형
                dto.Title = VocModel.Title; // 민원 제목
                dto.Contents = VocModel.Content; // 민원 제목
                dto.CreateUser = VocModel.CreateUser; // 민원 신청자 이름
                dto.Phone = VocModel.Phone; // 민원인 전화번호
                dto.CompleteDT = VocModel.CompleteDt; // 완료시간

                if (!String.IsNullOrWhiteSpace(VocModel.DurationDt))
                {
                    TimeSpan? DurationTime = TimeSpan.Parse(VocModel.DurationDt);
                    dto.DurationDT = $"{DurationTime.Value.Days}일{DurationTime.Value.Hours}시{DurationTime.Value.Minutes}분";
                }
                else
                {
                    dto.DurationDT = null;
                }

                string VocFileName = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, BuildingModel.PlaceTbId);
                if (!String.IsNullOrWhiteSpace(VocModel.Image1))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, VocModel.Image1);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(VocModel.Image2))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, VocModel.Image2);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(VocModel.Image3))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, VocModel.Image3);
                    dto.Images.Add(ImageBytes);
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
        public async ValueTask<ResponseList<VocCommentListDTO>?> GetVocCommentList(string? voccode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(voccode))
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoByCode(voccode);
                if(VocTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };

                List<CommentTb>? model = await VocCommentRepository.GetCommentList(VocTB.Id);
                if(model is [_, ..])
                {
                    return new ResponseList<VocCommentListDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model.Select(e => new VocCommentListDTO
                        {
                            Id = e.Id,
                            Comment = e.Content,
                            CreateDT = e.CreateDt,
                            CreateUser = e.CreateUser,
                            VocTbId = e.VocTbId
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<VocCommentListDTO>(), code = 200 };
                }
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
        public async ValueTask<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(int? commentid)
        {
            try
            {
                if (commentid is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                CommentTb? model = await VocCommentRepository.GetCommentInfo(commentid);
                if(model is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId);
                if(VocTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                PlaceTb? PlaceTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId);
                if (PlaceTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentDetailDTO dto = new VocCommentDetailDTO();
                dto.VocCommentId = model.Id; // VOC 댓글 ID
                dto.Content = model.Content; // VOC 댓글 내용
                dto.Status = model.Status; // VOC 댓글 상태
                dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                dto.CreateUser = model.CreateUser; // 댓글 작성자

                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, PlaceTB.Id);
                if (!String.IsNullOrWhiteSpace(model.Image1))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image1);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image2))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image2);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image3))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image3);
                    dto.Images.Add(ImageBytes);
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
