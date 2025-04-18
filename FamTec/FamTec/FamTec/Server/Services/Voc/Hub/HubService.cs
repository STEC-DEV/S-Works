using FamTec.Server.Helpers;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Server.Services.Redis;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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
        private readonly ILogService LogService;
        private readonly ConsoleLogService<HubService> CreateBuilderLogger;

        private readonly AuthCodeService AuthCodeService;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocFileFolderPath;
        private string? VocCommentFileFolderPath;

        private readonly IRedisService RedisService;

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
            ILogService _logservice,
            IRedisService _redisservice,
            ConsoleLogService<HubService> _createbuilderlogger,
            AuthCodeService _authcodeservice)
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
            this.CreateBuilderLogger = _createbuilderlogger;
            this.AuthCodeService = _authcodeservice;

            this.RedisService = _redisservice;
        }

        /// <summary>
        /// 인증코드 발급
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> AddAuthCodeService(int PlaceId, int BuildingId, string PhoneNumber)
        {
            try
            {
                if(String.IsNullOrWhiteSpace(PhoneNumber))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (!String.IsNullOrWhiteSpace(PhoneNumber))
                {
                    bool PhoneResult = long.TryParse(PhoneNumber, out _);
                    if (!PhoneResult)
                        return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }

                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(BuildingId).ConfigureAwait(false);
                if (buildingtb is null)
                    return new ResponseUnit<bool>() { message = "존재하지 않는 건물입니다.", data = false, code = 404 };

                DateTime ThisDate = DateTime.Now;

                // 랜덬코드 발급 - Redis로 변경
                string randomCode = await RedisService.SetCodeAsync(PhoneNumber);

                // 기존 Regacy
                //string randomCode = KakaoService.RandomVerifyAuthCode();

                // 카카도 API 전송
                bool SendResult = await KakaoService.AddVerifyAuthCodeAnser(buildingtb.Name, PhoneNumber, randomCode);

                // 인증코드 메모리 저장
                if (SendResult)
                {
                    //bool result = await AuthCodeService.SaveOrUpdateAuthCode(PhoneNumber, randomCode); // Memory Cache 사용
                    //return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = result, code = 200 }; // Memory Cache 사용
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 }; // Redis 사용
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 인증코드 검사
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> GetVerifyAuthCodeService(string PhoneNumber, string AuthCode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(PhoneNumber) || String.IsNullOrWhiteSpace(AuthCode))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (!String.IsNullOrWhiteSpace(PhoneNumber))
                {
                    bool PhoneResult = long.TryParse(PhoneNumber, out _);
                    if (!PhoneResult)
                        return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }

                //bool result = await AuthCodeService.CheckVerifyAuthCode(PhoneNumber, AuthCode); // Memory Cache 사용
                bool result = await RedisService.GetValidateCodeAsync(PhoneNumber, AuthCode); // Redis 사용
                if(result)
                {
                    return new ResponseUnit<bool>() { message = "인증 성공하였습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "인증 실패하였습니다.", data = false, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 민원 추가 V2
        /// - 알림톡 전송여부 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddVocReturnDTO?>> AddVocServiceV2(AddVocDTOV2 dto, List<IFormFile>? image)
        {
            try
            {
                if (dto is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 400 };

                if (String.IsNullOrEmpty(dto.title) || String.IsNullOrEmpty(dto.contents) || String.IsNullOrEmpty(dto.name))
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 400 };

                if (dto.buildingId == 0 || dto.placeId == 0)
                    return new ResponseUnit<AddVocReturnDTO?> { message = "잘못된 요청입니다.", code = 400 };

                if (!String.IsNullOrEmpty(dto.phoneNumber))
                {
                    if (!Regex.IsMatch(dto.phoneNumber ?? "", @"^010\d{8}$"))
                    {
                        return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 전화번호입니다.", data = null, code = 404 };
                    }
                }

                List<string> newFileName = new List<string>();

                if (image is not null)
                {
                    foreach(IFormFile file in image)
                    {
                        // 이미지명칭 새롭게 지정
                        newFileName.Add(FileService.SetNewFileName(dto.name!, file));
                    }
                }


                var buildingTB = await BuildingInfoRepository.GetBuildingInfo(dto.buildingId!).ConfigureAwait(false);
                if (buildingTB is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "존재하지 않는 건물입니다.", data = null, code = 404 };
              
                // 시스템 시간 조회
                DateTime ThisDate = DateTime.Now;

                // Receipt 코드(랜덤코드) 생성 + DB 저장 
                const int MaxAttempts = 5;
                int attempt = 0;
                
                VocTb SaveModel = null!;
                while(true)
                {
                    attempt++;

                    var model = new VocTb();
                    model.Title = dto.title;
                    model.Content = dto.contents;
                    model.Type = dto.type;
                    model.Phone = dto.phoneNumber;
                    model.Status = 0; // ADD -> 미처리임
                    model.Division = dto.division; // 모바일 : 0, 웹 : 1
                    model.CreateDt = ThisDate;
                    model.CreateUser = dto.name; // 작성자
                    model.UpdateDt = ThisDate;
                    model.UpdateUser = dto.name; // 작성자
                    model.BuildingTbId = dto.buildingId;
                    // replyYN 결정
                    model.ReplyYn = dto.division == 0 ? !String.IsNullOrEmpty(dto.phoneNumber) : (!String.IsNullOrEmpty(dto.phoneNumber) && dto.replyYn);

                    // 파일이 있으면
                    if (image is not null && image.Count > 0)
                    {
                        // 이미지 저장
                        for (int i = 0; i < image.Count(); i++)
                        {
                            switch (i)
                            {
                                case 0: model.Image1 = newFileName[i]; break; // 새로운 파일명칭 이름만 저장
                                case 1: model.Image2 = newFileName[i]; break; // 새로운 파일명칭 이름만 저장
                                case 2: model.Image3 = newFileName[i]; break; // 새로운 파일명칭 이름만 저장
                            }
                        }
                    }

                    model.Code = UniqueCodeHelpers.RandomCode();


                    try
                    {
                        SaveModel = await VocInfoRepository.AddAsync(model).ConfigureAwait(false) ?? throw new Exception("DB 저장 실패");
                        break;
                    }
                    catch(DbUpdateException ex) when (IsUniqueViolation(ex) && attempt < MaxAttempts)
                    {
                        // 중복 키 발생 : 재시도
                        continue;
                    }
                    catch(DbUpdateException ex) when (IsUniqueViolation(ex))
                    {
                        // 최대 재시도 초과
                        return new ResponseUnit<AddVocReturnDTO?>
                        {
                            message = "접수번호 생성에 실패했습니다. 잠시 후 다시 시도해주세요:",
                            data = null,
                            code = 503
                        };
                    }
                }

                var resultseq = SaveModel.Id;
                // 블랙리스트 & 알림톡 처리
                if(SaveModel.ReplyYn)
                {
                    /*
                    위의 로직을 수행하고 ReplyYN 유무를 검사해서 알림톡 수신을 받는다고 한 경우
                    BlackList인지 검사하고
                    BlackList 이면 Database에 ADD만 수행 - 위에서 함
                    BlackList가 아니면 API 전송
                    */
                    var BlackListTB = await BlackListInfoRepository.GetBlackListInfo(SaveModel.Phone).ConfigureAwait(false);
                    if(BlackListTB is null)
                    {
                        // 사업장 정보
                        var PlaceTB = await PlaceInfoRepository.GetBuildingPlace(SaveModel.BuildingTbId).ConfigureAwait(false);
                        if (PlaceTB is null)
                            return new ResponseUnit<AddVocReturnDTO?>() { message = "존재하지 않는 사업장입니다.", data = null, code = 404 };

                        // 알림톡
                        string title = SaveModel.Title.Length > 8 ? SaveModel.Title.Substring(0, 8) + "..." : SaveModel.Title;
                        string url = $"https://sws.s-tec.co.kr/m/voc/select/{SaveModel.Code}";

                        // 카카오 알림톡 정송
                        var ApiSendResult = await KakaoService.AddVocAnswer(title, SaveModel.Code, ThisDate, SaveModel.Phone, url, PlaceTB.Tel).ConfigureAwait(false);

                        if(ApiSendResult is not null)
                        {
                            // 카카오 메시지 성공 - 벤더사에 넘어가면 여기탐.
                            var KakaoLogTB = new KakaoLogTb();
                            KakaoLogTB.Code = ApiSendResult.Code;
                            KakaoLogTB.Message = ApiSendResult.Message;
                            KakaoLogTB.Msgid = ApiSendResult.MSGID;
                            KakaoLogTB.Phone = SaveModel.Phone; // 받는사람 전화번호
                            KakaoLogTB.MsgUpdate = false; // 카카오 API 벤더에서 전송결과 유무를 받아서 전송됐는지 확인위한 FLAG
                            KakaoLogTB.CreateDt = ThisDate;
                            KakaoLogTB.CreateUser = SaveModel.CreateUser;
                            KakaoLogTB.UpdateDt = ThisDate;
                            KakaoLogTB.UpdateUser = SaveModel.UpdateUser;
                            KakaoLogTB.VocTbId = SaveModel.Id;
                            KakaoLogTB.PlaceTbId = PlaceTB.Id;
                            KakaoLogTB.BuildingTbId = SaveModel.BuildingTbId;

                            var kakaoLogSaveResult = await KakaoLogInfoRepository.AddAsync(KakaoLogTB).ConfigureAwait(false);
                        }
                        else
                        {
                            var KakaoLogTB = new KakaoLogTb();
                            KakaoLogTB.Code = null;
                            KakaoLogTB.Message = "500";
                            KakaoLogTB.Msgid = null;
                            KakaoLogTB.Phone = SaveModel.Phone; // 받는사람 전화번호
                            KakaoLogTB.MsgUpdate = false; // 카카오 API 벤더에서 전송결과 유무를 받아서 전송됐는지 확인위한 FLAG
                            KakaoLogTB.CreateDt = ThisDate;
                            KakaoLogTB.CreateUser = SaveModel.CreateUser;
                            KakaoLogTB.UpdateDt = ThisDate;
                            KakaoLogTB.UpdateUser = SaveModel.UpdateUser;
                            KakaoLogTB.VocTbId = SaveModel.Id;
                            KakaoLogTB.PlaceTbId = PlaceTB.Id;
                            KakaoLogTB.BuildingTbId = SaveModel.BuildingTbId;

                            var kakaoLogSaveResult = await KakaoLogInfoRepository.AddAsync(KakaoLogTB).ConfigureAwait(false);
                        }
                    }
                }

                // 파일 저장
                // 7. 파일 저장
                if (image?.Any() ?? false)
                {
                    // 그리고 여기에서 진짜 파일저장하고
                    VocFileFolderPath = Path.Combine(Common.FileServer, dto.placeId.ToString(), "Voc", resultseq.ToString());

                    di = new DirectoryInfo(VocFileFolderPath);
                    if (!di.Exists) di.Create();

                    for (int i = 0; i < image.Count; i++)
                    {
                        await FileService.AddResizeImageFile(newFileName[i], VocFileFolderPath, image[i]).ConfigureAwait(false);
                    }
                }

                // SignalR 쏘고
                // 민원등록 알림
                await HubContext.Clients.Group($"{dto.placeId}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다").ConfigureAwait(false);
                // 민원 상태변경 알림 - 대쉬보드
                await HubContext.Clients.Group($"{dto.placeId}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);

                // 9. 최종 응답
                return new ResponseUnit<AddVocReturnDTO?>
                {
                    message = "요청이 정상 처리되었습니다.",
                    data = new AddVocReturnDTO
                    {
                        ReceiptCode = SaveModel.Code,
                        PhoneNumber = SaveModel.Phone,
                        CreateDT = SaveModel.CreateDt.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    code = 200
                };
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddVocReturnDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다. 잠시 후 다시 시도해주세요", data = null, code = 503 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddVocReturnDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        

        /// <summary>
        /// 민원 추가 - 민원인 전용
        /// </summary>
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
                    bool PhoneNumber = long.TryParse(dto.PhoneNumber, out _);
                    if(!PhoneNumber)
                        return new ResponseUnit<AddVocReturnDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                DateTime ThisDate = DateTime.Now;

                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(dto.Buildingid!.Value).ConfigureAwait(false);
                if (buildingtb is null)
                    return new ResponseUnit<AddVocReturnDTO?>() { message = "존재하지 않는 건물입니다.", data = null, code = 404 };

                VocTb? model = new VocTb();
                model.Title = dto.Title;
                model.Content = dto.Contents;
                model.Type = dto.Type; // 처음은 미분류
                model.Phone = dto.PhoneNumber; // 전화번호
                model.Status = 0; // 미처리
                model.Division = dto.Division;

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
                    VocFileFolderPath = Path.Combine(Common.FileServer, dto.Placeid.ToString(), "Voc", result.Id.ToString());

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
                        //byte[] bytes = Encoding.Unicode.GetBytes(result.Id.ToString());
                        //string base64 = Convert.ToBase64String(bytes);

                        string url = $"https://sws.s-tec.co.kr/m/voc/select/{result.Code}";

                        // 카카오 API 전송
                        // 보낸 USER 휴대폰번호에 전송.
                        AddKakaoLogDTO? LogDTO = await KakaoService.AddVocAnswer(Title, model.Code, DateNow, receiver, url, placetel).ConfigureAwait(false);
                        if (LogDTO is not null)
                        {
                            // 카카오 메시지 성공 - 벤더사에 넘어가기만 하면 여기탐
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = LogDTO.Code!;
                            LogTB.Message = LogDTO.Message!;
                            LogTB.Msgid = LogDTO.MSGID;
                            LogTB.Phone = LogDTO.Phone; // 받는사람 전화번호
                            LogTB.MsgUpdate = false;
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
                        else // 404 에러시에 여기가탈듯.
                        {
                            // 카카오 메시지 에러
                            KakaoLogTb LogTB = new KakaoLogTb();
                            LogTB.Code = null;
                            LogTB.Message = "500";
                            LogTB.Msgid = null;
                            LogTB.Phone = receiver; // 받는사람 전화번호
                            LogTB.MsgUpdate = true;
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
                        // VOCTYPE = 0 >> 미분류
                        await AlarmInfoRepository.AddAlarmList(UserList, dto.Name, 0, result.Id, 0).ConfigureAwait(false);
                    }


                    // 민원등록 알림
                    await HubContext.Clients.Group($"{dto.Placeid}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다").ConfigureAwait(false);
                    // 민원 상태변경 알림 - 대쉬보드
                    await HubContext.Clients.Group($"{dto.Placeid}_VocStatus").SendAsync("ReceiveVocStatus", "민원의 상태가 변경되었습니다.").ConfigureAwait(false);


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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddVocReturnDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원 조회 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<VocUserDetailDTO?>> GetVocRecord(string? voccode, bool isMobile)
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

                VocFileFolderPath = Path.Combine(Common.FileServer, BuildingModel.PlaceTbId.ToString(), "Voc", VocModel.Id.ToString());

                di = new DirectoryInfo(VocFileFolderPath);
                if (!di.Exists) di.Create();

                if (isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                    // 모바일
                    var imageFiles = new[] { VocModel.Image1, VocModel.Image2, VocModel.Image3 };

                    foreach (var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileFolderPath, image).ConfigureAwait(false);
                            if (ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if (files is not null)
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

                    return new ResponseUnit<VocUserDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    var imageFiles = new[] { VocModel.Image1, VocModel.Image2, VocModel.Image3 };
                    foreach (var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image)) // 이미지명칭이 DB에 있으면
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileFolderPath, image).ConfigureAwait(false);

                            if(ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if(files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

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
                        else // 이미지 명칭에 DB에 없으면.
                        {
                            dto.ImageName.Add(null);
                            dto.Images.Add(null);
                        }
                    }

                    return new ResponseUnit<VocUserDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<VocUserDetailDTO?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      /// <summary>
      /// 민원 댓글조회 - 민원인 전용
      /// </summary>
      /// <returns></returns>
      public async Task<ResponseList<VocCommentListDTO>?> GetVocCommentList(string? voccode, bool isMobile)
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

              VocCommentFileFolderPath = Path.Combine(Common.FileServer, BuildingTB!.PlaceTbId.ToString(), "Voc", VocTB.Id.ToString(), "VocComment");

              di = new DirectoryInfo(VocCommentFileFolderPath);
              if (!di.Exists) di.Create();

                if(isMobile) // 모바일
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
                        dtoModel.CreateUserId = model[i].UserTbId; // 민원댓글 생성자 ID
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

                    return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
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
                        dtoModel.CreateUserId = model[i].UserTbId; // 민원댓글 생성자 ID
                        dtoModel.CreateUser = model[i].CreateUser;
                        dtoModel.VocTbId = model[i].VocTbId;

                        var imageFiles = new[] { model[i].Image1, model[i].Image2, model[i].Image3 };
                        
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
                                        byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);
                                        if(ConvertFile is not null)
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
                    
                    return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<VocCommentListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원댓글 상세보기 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(int? commentid, bool isMobile)
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

                VocCommentFileFolderPath = Path.Combine(Common.FileServer, PlaceTB.Id.ToString(), "Voc", VocTB.Id.ToString(), "VocComment");

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                if (isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("====모바일====");
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
                    foreach(var image in imageFiles)
                    {
                        if (!String.IsNullOrWhiteSpace(image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, image).ConfigureAwait(false);

                            if(ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, image);
                                if(files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

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
        /// MySQL(1062) 또는 SQL Server(2601/2627) 유니크 위반 여부 검사
        /// </summary>
        private bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is MySqlConnector.MySqlException m && m.Number == 1062)
                return true;
            if (ex.InnerException is System.Data.SqlClient.SqlException s
                && (s.Number == 2601 || s.Number == 2627))
                return true;
            return false;
        }

    }
}
