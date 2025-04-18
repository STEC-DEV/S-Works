﻿using FamTec.Client.Pages.Normal.User.UserAdd;
using FamTec.Server.Hubs;
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
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Data.SqlClient;

namespace FamTec.Server.Services.Voc
{
    public class VocCommentService : IVocCommentService
    {
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IUserInfoRepository UserInfoRepository;
        
        private readonly IKakaoService KakaoService;

        private readonly ILogService LogService;
        private readonly IFileService FileService;
        private readonly ConsoleLogService<VocCommentService> CreateBuilderLogger;

        private readonly IHubContext<BroadcastHub> HubContext;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocCommentFileFolderPath;

        public VocCommentService(IVocCommentRepository _voccommentrepository,
            IVocInfoRepository _vocinforepository,
            IBlackListInfoRepository _blacklistinforepository,
            IPlaceInfoRepository _placeinforepository,
            IKakaoLogInfoRepository _kakaologinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IUserInfoRepository _userinforepository,
            IKakaoService _kakaoservice,
            ILogService _logservice,
            IFileService _fileservice,
            IHubContext<BroadcastHub> _hubcontext,
            ConsoleLogService<VocCommentService> _createbuilderlogger)
        {
            this.VocCommentRepository = _voccommentrepository;
            this.VocInfoRepository = _vocinforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.UserInfoRepository = _userinforepository;

            this.KakaoService = _kakaoservice;
            this.HubContext = _hubcontext;

            this.LogService = _logservice;
            this.FileService = _fileservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }


        /// <summary>
        /// 민원 댓글 추가 - v2
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddVocCommentDTOV2?>> AddVocCommentServiceV2(HttpContext context, AddVocCommentDTOV2 dto, List<IFormFile> image)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "요청 권한이 없습니다.", data = null, code = 401 };

                if (dto is null)
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "잘못된 요청입니다.", data = null, code = 204 };

                if (String.IsNullOrWhiteSpace(dto.Content))
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "공백을 등록할 수 없습니다.", data = null, code = 204 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creator = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeId) || String.IsNullOrWhiteSpace(Creator) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "요청 권한이 없습니다.", data = null, code = 401 };

                List<string> newFileName = new List<string>();

                if(image is not null)
                {
                    foreach(IFormFile file in image)
                    {
                        // 이미지명칭 새롭게 지정
                        newFileName.Add(FileService.SetNewFileName(UserIdx, file));
                    }
                }

                // 현재시간 저장
                DateTime ThisDate = DateTime.Now;

                CommentTb? model = new CommentTb();
                model.Content = dto.Content;
                model.Status = dto.Status!.Value; // 민원의 상태
                model.KakaosendYn = dto.sendYn; // 알림톡 발송여부
                model.CreateDt = ThisDate; // 현재시간
                model.CreateUser = Creator; // 작성자
                model.UpdateDt = ThisDate; // 수정시간
                model.UpdateUser = Creator; // 수정자 (작성자와 동일)
                model.VocTbId = dto.VocTbId!.Value; // 민원 ID
                model.UserTbId = Convert.ToInt32(UserIdx);

                // 파일이 있으면
                if(image is not null && image.Count > 0)
                {
                    for(int i = 0; i < image.Count(); i++)
                    {
                        switch(i)
                        {
                            case 0: model.Image1 = newFileName[i]; break;
                            case 1: model.Image2 = newFileName[i]; break;
                            case 2: model.Image3 = newFileName[i]; break;
                            default: break;
                        }
                    }
                }

                /* 민원 댓글 추가 */
                CommentTb? CommentTB = await VocCommentRepository.AddAsync(model).ConfigureAwait(false);
                if (CommentTB is null)
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

                /* 민원의 상태를 변경하기 위해 조회해온다. */
                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId).ConfigureAwait(false);
                if (VocTB is null)
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "존재하지 않는 민원내용 입니다.", data = null, code = 204 };

                // VocTB == 2
                if(VocTB.Status == 2)
                {
                    VocTB.CompleteDt = CommentTB.CreateDt; // 코맨트 등록시간을 VOC 완료시간으로
                    VocTB.DurationDt = (CommentTB.CreateDt - VocTB.CreateDt).ToString(); // 댓글등록시간 - VOC 등록시간 => 소요시간
                }

                VocTB.Status = dto.Status.Value;
                VocTB.UpdateDt = ThisDate;
                VocTB.UpdateUser = Creator;
                bool VocUpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB).ConfigureAwait(false);

                if(VocUpdateResult) // VOC 상태 Update가 성공하였을 경우
                {
                    if (dto.sendYn) // 알림톡 발송하기로 체크한 경우
                    {
                        if(VocTB.ReplyYn == true && !String.IsNullOrWhiteSpace(VocTB.Phone)) // 처리결과를 받는다고 했었던 경우.
                        {
                            PlaceTb? PlaceTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId).ConfigureAwait(false);
                            if (PlaceTB is null)
                                return new ResponseUnit<AddVocCommentDTOV2?>() { message = "존재하지 않는 건물입니다.", data = null, code = 401 };

                            string receiver = VocTB.Phone; // 받는사람 전화번호
                            string placetel = PlaceTB.Tel; // 사업장 전화번호

                            string url = $"https://sws.s-tec.co.kr/m/voc/select/{VocTB.Code}";

                            string statusResult = String.Empty;
                            if(CommentTB.Status == 1)
                            {
                                statusResult = "처리중";
                            }
                            else if(CommentTB.Status == 2)
                            {
                                statusResult = "처리완료";
                            }

                            AddKakaoLogDTO? ApiSendResult = await KakaoService.UpdateVocAnswer(VocTB.Code, statusResult, receiver, url, placetel).ConfigureAwait(false);
                            if(ApiSendResult is not null)
                            {
                                // 카카오 메시지 성공 - 벤더사에 넘어가면 여기 탐
                                KakaoLogTb LogTB = new KakaoLogTb();
                                LogTB.Code =ApiSendResult.Code;
                                LogTB.Message = ApiSendResult.Message;
                                LogTB.Msgid = ApiSendResult.MSGID;
                                LogTB.Phone = ApiSendResult.Phone; // 받는사람 전화번호
                                LogTB.MsgUpdate = false; // 카카오 API 벤더에서 전송결과 유무를 받아서 전송됐는지 확인하기 위한 FLAG
                                LogTB.CreateDt = ThisDate; // 현재시간
                                LogTB.CreateUser = Creator; // 작성자
                                LogTB.UpdateDt = ThisDate; // 현재시간
                                LogTB.UpdateUser = Creator; // 작성자
                                LogTB.VocTbId = VocTB.Id; // 민원 ID
                                LogTB.PlaceTbId = PlaceTB.Id; // 사업장 ID
                                LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물 ID

                                var LogTBResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                                if (LogTBResult is null)
                                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
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
                                LogTB.UpdateUser = Creator;
                                LogTB.VocTbId = VocTB.Id; // 민원 ID
                                LogTB.PlaceTbId = PlaceTB.Id; // 사업장 ID
                                LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물 ID

                                var LogTBResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                                if (LogTBResult is null)
                                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                            }
                        }
                    }
                    else // 아닌경우
                    {
                        if(VocTB.ReplyYn == true && !String.IsNullOrWhiteSpace(VocTB.Phone)) // 처리결과를 받는다고 했었으면 전송해야함.
                        {
                            // 아니면 카카오 API + DB저장
                            PlaceTb? PlaceTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId).ConfigureAwait(false);
                            if (PlaceTB is null)
                                return new ResponseUnit<AddVocCommentDTOV2?>() { message = "존재하지 않는 건물입니다.", data = null, code = 401 };

                            string receiver = VocTB.Phone; // 받는사람 전화번호
                            string placetel = PlaceTB.Tel; // 사업장 전화번호

                            string url = $"https://sws.s-tec.co.kr/m/voc/select/{VocTB.Code}";

                            string statusResult = String.Empty;
                            if(CommentTB.Status == 1)
                            {
                                statusResult = "처리중";
                            }
                            else if(CommentTB.Status == 2)
                            {
                                statusResult = "처리완료";
                            }

                            AddKakaoLogDTO? ApiSendResult = await KakaoService.UpdateVocAnswer(VocTB.Code, statusResult, receiver, url, placetel).ConfigureAwait(false);
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
                                LogTB.CreateUser = Creator; // 작성자
                                LogTB.UpdateDt = ThisDate; // 현재시간
                                LogTB.UpdateUser = Creator; // 작성자
                                LogTB.VocTbId = VocTB.Id; // 민원ID
                                LogTB.PlaceTbId = PlaceTB.Id; // 사업장 ID
                                LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물 ID

                                var LogTBResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                                if (LogTBResult is null)
                                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
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
                                if (LogTBResult is null)
                                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                            }
                        }
                    }
                }
                else // 실패 하였을 경우
                {
                    return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }

                // 파일저장 해야함.
                // VOC 관련한 폴더 없으면 만들기
                VocCommentFileFolderPath = Path.Combine(Common.FileServer, placeId.ToString(), "Voc", dto.VocTbId!.Value.ToString(), "VocComment");

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                // 파일 넣기
                if (image is not null)
                {
                    for (int i = 0; i < image.Count; i++)
                    {
                        await FileService.AddResizeImageFile(newFileName[i], VocCommentFileFolderPath, image[i]).ConfigureAwait(false);
                    }
                }

                // SIGNAL R 전송
                // 민원 상태변경 알림 - 대쉬보드
                await HubContext.Clients.Group($"{placeId}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                return new ResponseUnit<AddVocCommentDTOV2?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddVocCommentDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddVocCommentDTOV2(), code = 500 };
            }
        }

        /// <summary>
        /// 민원 댓글 추가 [Regacy]
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<AddVocCommentDTO?>> AddVocCommentService(HttpContext context, AddVocCommentDTO dto, List<IFormFile> files)
        {
            try
            {
                List<string> newFileName = new List<string>();

                if (context is null || dto is null)
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);
                string? Useridx = Convert.ToString(context.Items["UserIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(placeId) || String.IsNullOrWhiteSpace(Creater) || String.IsNullOrWhiteSpace(Useridx))
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                //BlacklistTb? BlackListTB = await BlackListInfoRepository.GetBlackListInfo(VocTB.Phone!);
                //if (BlackListTB is not null)
                //    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 파일명 생성
                if (files is not null)
                {
                    foreach(IFormFile file in files)
                    {
                        newFileName.Add(FileService.SetNewFileName(Useridx, file));
                    }
                }

                // VOC관련한 폴더 없으면 만들기
                //VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, dto.VocTbId!.Value);
                VocCommentFileFolderPath = Path.Combine(Common.FileServer, placeId.ToString(), "Voc", dto.VocTbId!.Value.ToString(), "VocComment");

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                CommentTb? comment = new CommentTb();
                comment.Content = dto.Content!;
                comment.Status = dto.Status!.Value;
                comment.CreateDt = ThisDate;
                comment.CreateUser = Creater;
                comment.UpdateDt = ThisDate;
                comment.UpdateUser = Creater;
                comment.VocTbId = dto.VocTbId!.Value;
                comment.UserTbId = Convert.ToInt32(Useridx);

                // 파일이 있으면
                if (files is not null && files.Count > 0)
                {
                    // 이미지 저장
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (i is 0) comment.Image1 = newFileName[i];
                        if (i is 1) comment.Image2 = newFileName[i];
                        if (i is 2) comment.Image3 = newFileName[i];
                    }
                }

                CommentTb? model = await VocCommentRepository.AddAsync(comment).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (files is not null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        await FileService.AddResizeImageFile(newFileName[i], VocCommentFileFolderPath, files[i]).ConfigureAwait(false);
                    }
                }

                // 등록했으면 원래꺼 상태변경
                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId).ConfigureAwait(false);
                if(VocTB is null)
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(model.Status == 2) // 처리완료
                {
                    VocTB.CompleteDt = model.CreateDt; // 코맨트 등록시간을 VOC 완료시간으로
                    VocTB.DurationDt = (model.CreateDt - VocTB.CreateDt).ToString(); // 댓글등록시간 - VOC 등록시간 ==> 소요시간
                }

                VocTB.Status = dto.Status.Value;
                VocTB.UpdateDt = ThisDate;
                VocTB.UpdateUser = Creater;
                bool VocUpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB).ConfigureAwait(false);

                if (VocUpdateResult)
                {
                    // 만약 처리결과를 받는다고 했었으면.
                    if(VocTB.ReplyYn == true)
                    {
                        PlaceTb? placeTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId).ConfigureAwait(false);

                        if (placeTB is null)
                            return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                        // 전화번호
                        string receiver = VocTB.Phone!;

                        // 사업장 전화번호
                        string placetel = placeTB.Tel!;

                        // URL 파라미터 인코딩
                        //byte[] bytes = Encoding.Unicode.GetBytes(VocTB.Id.ToString());
                        //string base64 = Convert.ToBase64String(bytes);


                        // 템블릿 http로 새로 생성해야함.
                        //string url = $"http://125.131.105.172:5245/m/voc/select/{VocTB.Code}";
                        string url = $"https://sws.s-tec.co.kr/m/voc/select/{VocTB.Code}";

                        string StatusResult = string.Empty;
                        if(model.Status == 1)
                        {
                            StatusResult = "처리중";
                        }
                        else if(model.Status == 2)
                        {
                            StatusResult = "처리완료";
                        }

                        // 카카오 API 전송
                        AddKakaoLogDTO? LogDTO = await KakaoService.UpdateVocAnswer(VocTB.Code!, StatusResult, receiver, url, placetel).ConfigureAwait(false);

                        if(LogDTO is not null)
                        {
                            // 카카오 메시지 성공
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = LogDTO.Code!;
                            LogTB.Message = LogDTO.Message!;
                            LogTB.Msgid = LogDTO.MSGID;
                            LogTB.Phone = LogDTO.Phone; // 받는사람 전화번호
                            LogTB.MsgUpdate = false;
                            LogTB.CreateDt = ThisDate;
                            LogTB.CreateUser = Creater;
                            LogTB.UpdateDt = ThisDate;
                            LogTB.UpdateUser = Creater;
                            LogTB.VocTbId = VocTB.Id; // 민원ID
                            LogTB.PlaceTbId = placeTB.Id; // 사업장ID
                            LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물ID

                            // 카카오API 로그 테이블에 쌓아야함
                            KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                        }
                        else
                        {
                            // 카카오 메시지 에러
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = null;
                            LogTB.Message = "500";
                            LogTB.Msgid = null;
                            LogTB.Phone = receiver; // 받는사람 전화번호
                            LogTB.MsgUpdate = true;
                            LogTB.CreateDt = ThisDate;
                            LogTB.CreateUser = Creater;
                            LogTB.UpdateDt = ThisDate;
                            LogTB.UpdateUser = Creater;
                            LogTB.VocTbId = VocTB.Id; // 민원ID
                            LogTB.PlaceTbId = placeTB.Id; // 사업장ID
                            LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물ID

                            // 카카오API 로그 테이블에 쌓아야함
                            await KakaoLogInfoRepository.AddAsync(LogTB).ConfigureAwait(false);
                        }
                    }

                    // 민원 상태변경 알림 - 대쉬보드
                    await HubContext.Clients.Group($"{placeId}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);

                    return new ResponseUnit<AddVocCommentDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto , code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddVocCommentDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddVocCommentDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 민원에 대한 댓글 리스트 조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async Task<ResponseList<VocCommentListDTO>> GetVocCommentList(HttpContext context, int vocid, bool isMobile)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(vocid).ConfigureAwait(false);
                if (VocTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 404 };

                List<CommentTb>? model = await VocCommentRepository.GetCommentList(vocid).ConfigureAwait(false);
                if(model is null || model.Count == 0)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<VocCommentListDTO>(), code = 200 };

                BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB.BuildingTbId).ConfigureAwait(false);
                if(BuildingTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                //VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, BuildingTB!.PlaceTbId, VocTB.Id);
                VocCommentFileFolderPath = Path.Combine(Common.FileServer, BuildingTB!.PlaceTbId.ToString(), "Voc", VocTB.Id.ToString(), "VocComment");

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                if (isMobile) // 모바일
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif

                    List<VocCommentListDTO> dto = new List<VocCommentListDTO>();

                    for (int i = 0; i < model.Count; i++)
                    {
                        VocCommentListDTO dtoModel = new VocCommentListDTO();
                        dtoModel.Id = model[i].Id;
                        dtoModel.status = model[i].Status; // 댓글상태
                        dtoModel.Comment = model[i].Content; // 내용
                        dtoModel.CreateDT = model[i].CreateDt.ToString("yyyy-MM-dd HH:mm:ss");
                        dtoModel.CreateUserId = model[i].UserTbId;
                        dtoModel.CreateUser = model[i].CreateUser;
                        dtoModel.VocTbId = model[i].VocTbId;

                        var imageFiles = new[] { model[i].Image1, model[i].Image2, model[i].Image3 };
                        foreach (var image in imageFiles)
                        {
                            if (!String.IsNullOrWhiteSpace(image))
                            {
                                byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);
                                if (ImageBytes is not null)
                                {
                                    IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                    if (files is not null)
                                    {
                                        byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                        if (ConvertFile is not null)
                                        {
                                            dtoModel.ImageName.Add(image);
                                            dtoModel.Images.Add(ConvertFile);
                                        }
                                        else
                                        {
                                            dtoModel.ImageName.Add(null);
                                            dtoModel.Images.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        dtoModel.ImageName.Add(null);
                                        dtoModel.Images.Add(null);
                                    }
                                }
                                else
                                {
                                    dtoModel.ImageName.Add(null);
                                    dtoModel.Images.Add(null);
                                }
                            }
                            else
                            {
                                dtoModel.ImageName.Add(null);
                                dtoModel.Images.Add(null);
                            }
                        }
                        dto.Add(dtoModel);
                    }
                    return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto.OrderByDescending(m => m.CreateDT).ToList(), code = 200 };
                }
                else // PC
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    List<VocCommentListDTO> dto = new List<VocCommentListDTO>();
                    for (int i = 0; i < model.Count; i++)
                    {
                        VocCommentListDTO dtoModel = new VocCommentListDTO();
                        dtoModel.Id = model[i].Id;
                        dtoModel.status = model[i].Status; // 댓글상태
                        dtoModel.Comment = model[i].Content; // 내용
                        dtoModel.CreateDT = model[i].CreateDt.ToString("yyyy-MM-dd HH:mm:ss");
                        dtoModel.CreateUserId = model[i].UserTbId;
                        dtoModel.CreateUser = model[i].CreateUser;
                        dtoModel.VocTbId = model[i].VocTbId;

                        var imageFiles = new[] { model[i].Image1, model[i].Image2, model[i].Image3 };
                        foreach (var image in imageFiles)
                        {
                            if (!String.IsNullOrWhiteSpace(image))
                            {
                                byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);
                                if (ImageBytes is not null)
                                {
                                    IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                    if (files is not null)
                                    {
                                        byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

                                        if (ConvertFile is not null)
                                        {
                                            dtoModel.ImageName.Add(image);
                                            dtoModel.Images.Add(ConvertFile);
                                        }
                                        else
                                        {
                                            dtoModel.ImageName.Add(null);
                                            dtoModel.Images.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        dtoModel.ImageName.Add(null);
                                        dtoModel.Images.Add(null);
                                    }
                                }
                                else
                                {
                                    dtoModel.ImageName.Add(null);
                                    dtoModel.Images.Add(null);
                                }
                            }
                            else
                            {
                                dtoModel.ImageName.Add(null);
                                dtoModel.Images.Add(null);
                            }
                        }

                        dto.Add(dtoModel);
                    }

                    return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto.OrderByDescending(m => m.CreateDT).ToList(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocCommentListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocCommentListDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 민원에 대한 댓글 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(HttpContext context, int commentid, bool isMobile)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                CommentTb? model = await VocCommentRepository.GetCommentInfo(commentid).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId).ConfigureAwait(false);
                if (VocTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentDetailDTO dto = new VocCommentDetailDTO();
                dto.VocCommentId = model.Id; // VOC 댓글 ID
                dto.Content = model.Content; // VOC 댓글 내용
                dto.Status = model.Status; // VOC 댓글 상태
                dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                dto.CreateUser = model.CreateUser; // 댓글 작성자

                //VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, VocTB.Id);
                VocCommentFileFolderPath = Path.Combine(Common.FileServer, placeId.ToString(), "Voc", VocTB.Id.ToString(), "VocComment");

                if (isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                    var imageFiles = new[] { model.Image1, model.Image2, model.Image3 };
                    foreach(var image in imageFiles)
                    {
                        if(!String.IsNullOrWhiteSpace(image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);

                            if(ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if(files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                    if(ConvertFile is not null)
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
                        else
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    var imageFiles = new[] { model.Image1, model.Image2, model.Image3 };
                    foreach (var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);

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
                        }
                        else
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }

                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<VocCommentDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 차후 바꿈
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateCommentService(HttpContext context, VocCommentDetailDTO dto, List<IFormFile>? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? Useridx = Convert.ToString(context.Items["UserIdx"]);
                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(Useridx) || String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 내가 쓴건지 확인
                CommentTb? model = await VocCommentRepository.GetCommentInfo(dto.VocCommentId!.Value).ConfigureAwait(false);
                if(model!.UserTbId != Convert.ToInt32(Useridx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Content = dto.Content!;
                model.UpdateDt = ThisDate;
                model.UpdateUser = creater;


                // 파일처리 준비
                List<string> NewFileName = new List<string>();
                List<string> deleteFileName = new List<string>();

                // 수정실패 시 돌려놓을 FormFile 들
                // - 얘는 실패시 생성해야함.
                List<IFormFile?> AddTemplist = new List<IFormFile?>();
                // - 얘는 실패시 삭제해야함.
                List<string> RemoveTemplist = new List<string>();

                // VOC관련한 폴더 경로
                //VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, model.VocTbId);
                VocCommentFileFolderPath = Path.Combine(Common.FileServer, placeId.ToString(), "Voc", model.VocTbId.ToString(), "VocComment");

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                if (files is not null && files.Any()) //파일이 공백이 아닌경우
                {
                    for(int i=0;i<files.Count();i++)
                    {
                        var file = files[i];

                        if(i == 0) // Image1에 대한 처리
                        {
                            if(file.FileName != model.Image1)
                            {
                                if(!String.IsNullOrWhiteSpace(model.Image1))
                                {
                                    deleteFileName.Add(model.Image1);
                                }

                                // 새로운 파일명 설정
                                string newFileName = FileService.SetNewFileName(Useridx, file);
                                NewFileName.Add(newFileName); // 파일명 리스트에 추가
                                model.Image1 = newFileName;   // Image1 업데이트

                                RemoveTemplist.Add(newFileName);

                            }

                            // 넘어온 파일이 1개일 경우 Image2와 Image3를 비움.
                            if(files.Count == 1)
                            {
                                if(!String.IsNullOrWhiteSpace(model.Image2))
                                {
                                    deleteFileName.Add(model.Image2);
                                    model.Image2 = null;
                                }
                                if(!String.IsNullOrWhiteSpace(model.Image3))
                                {
                                    deleteFileName.Add(model.Image3);
                                    model.Image3 = null;
                                }
                            }
                        }
                        else if(i == 1 && files.Count > 1) // Image2에 대한 처리 (파일이 2개일때만)
                        {
                            if(file.FileName != model.Image2)
                            {
                                if(!String.IsNullOrWhiteSpace(model.Image2))
                                {
                                    deleteFileName.Add(model.Image2);
                                }

                                string newFileName = FileService.SetNewFileName(Useridx, file);
                                NewFileName.Add(newFileName); // 파일명 리스트에 추가
                                model.Image2 = newFileName;   // Image2 업데이트

                                RemoveTemplist.Add(newFileName);
                            }

                            if(files.Count == 2)
                            {
                                if(!String.IsNullOrWhiteSpace(model.Image3))
                                {
                                    deleteFileName.Add(model.Image3);
                                    model.Image3 = null;
                                }
                            }
                        }
                        else if(i == 2 && files.Count > 2) // Image3에 대한 처리 (파일이 3개일떄만)
                        {
                            if(file.FileName != model.Image3)
                            {
                                if(!String.IsNullOrWhiteSpace(model.Image3))
                                {
                                    deleteFileName.Add(model.Image3);
                                }
                                string newFileName = FileService.SetNewFileName(Useridx, file);
                                NewFileName.Add(newFileName); // 파일명 리스트에 추가
                                model.Image3 = newFileName;   // Image3 업데이트

                                RemoveTemplist.Add(newFileName);
                            }
                        }
                    }
                }
                else // 파일이 공백인 경우
                {
                    if (!String.IsNullOrWhiteSpace(model.Image1))
                    {
                        deleteFileName.Add(model.Image1); // 기존 파일 삭제 목록에 추가
                        model.Image1 = null; // 모델의 파일명 비우기
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image2))
                    {
                        deleteFileName.Add(model.Image2); // 기존 파일 삭제 목록에 추가
                        model.Image2 = null; // 모델의 파일명 비우기
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image3))
                    {
                        deleteFileName.Add(model.Image3); // 기존 파일 삭제 목록에 추가
                        model.Image3 = null; // 모델의 파일명 비우기
                    }
                }

                
                // 먼저 파일 삭제 처리
                for (int i = 0; i < deleteFileName.Count; i++) // 삭제할 파일 처리
                {
                    // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환해서 가지고있어야함.
                    byte[]? temp = await FileService.GetImageFile(VocCommentFileFolderPath, deleteFileName[i]).ConfigureAwait(false);
                    // - DB 실패했을경우 IFormFile을 바이트로 변환해서 DB의 해당명칭으로 다시저장해야함.
                    if (temp is not null)
                    {
                        AddTemplist.Add(FileService.ConvertFormFiles(temp, deleteFileName[i]));
                    }

                    FileService.DeleteImageFile(VocCommentFileFolderPath, deleteFileName[i]); // 파일 삭제
                }

                // 새 파일 저장
                if (files is not null && files.Any())
                {
                    for (int i = 0; i < files.Count(); i++) // 새 파일을 저장
                    {
                        var file = files[i];
                        if (i == 0) // Image1 처리
                        {
                            if (string.IsNullOrEmpty(model.Image1) || file.FileName != model.Image1)
                            {
                                // Image1이 없거나 기존 파일명과 다를 경우에만 파일 저장
                                await FileService.AddResizeImageFile(model.Image1!, VocCommentFileFolderPath, file).ConfigureAwait(false);
                            }
                        }
                        else if (i == 1 && files.Count > 1) // Image2 처리
                        {
                            if (string.IsNullOrEmpty(model.Image2) || file.FileName != model.Image2)
                            {
                                // Image2가 없거나 기존 파일명과 다를 경우에만 파일 저장
                                await FileService.AddResizeImageFile(model.Image2!, VocCommentFileFolderPath, file).ConfigureAwait(false);
                            }
                        }
                        else if (i == 2 && files.Count > 2) // Image3 처리
                        {
                            if (string.IsNullOrEmpty(model.Image3) || file.FileName != model.Image3)
                            {
                                // Image3이 없거나 기존 파일명과 다를 경우에만 파일 저장
                                await FileService.AddResizeImageFile(model.Image3!, VocCommentFileFolderPath, file).ConfigureAwait(false);
                            }
                        }
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? UpdateResult = await VocCommentRepository.UpdateCommentInfo(model).ConfigureAwait(false);
                if (UpdateResult == true)
                {
                    // 민원 상태변경 알림 - 대쉬보드
                    await HubContext.Clients.Group($"{placeId}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 여기서 파일 돌려놔야함.
                    if (AddTemplist.Count > 0)
                    {
                        for (int i = 0; i < AddTemplist.Count; i++)
                        {
                            try
                            {
                                // 파일이 존재하지 않으면 저장
                                if(FileService.IsFileExists(VocCommentFileFolderPath, AddTemplist[i].FileName) == false)
                                {
                                    // 파일을 저장하는 로직 (AddResizeImageFile)
                                    await FileService.AddResizeImageFile(AddTemplist[i].FileName, VocCommentFileFolderPath, AddTemplist[i]).ConfigureAwait(false);
                                }
                            }
                            catch(Exception ex)
                            {
                                LogService.LogMessage($"파일 복원실패 : {ex.Message}");
                            }
                        }
                    }

                    if (RemoveTemplist is [_, ..])
                    {
                        for (int i = 0; i < RemoveTemplist.Count; i++)
                        {
                            try
                            {
                                FileService.DeleteImageFile(VocCommentFileFolderPath, RemoveTemplist[i]);
                            }
                            catch(Exception ex)
                            {
                                LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
                            }
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 404 };
                }
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

      
    }
}