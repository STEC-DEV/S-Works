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
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace FamTec.Server.Services.Voc
{
    public class VocService : IVocService
    {
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IAlarmInfoRepository AlarmInfoRepository;
        private readonly IFileService FileService;
        private readonly IUserInfoRepository UserInfoRepository;

        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;


        private readonly IKakaoService KakaoService;

        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IHubContext<BroadcastHub> HubContext;


        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocFileFolderPath;
        private ILogService LogService;

        public VocService(IVocInfoRepository _vocinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IAlarmInfoRepository _alarminforepository,
            IUserInfoRepository _userinforepository,
            IPlaceInfoRepository _placeinforepository,
            IBlackListInfoRepository _blacklistinforepository,
            IKakaoLogInfoRepository _kakaologinforepository,
            IHubContext<BroadcastHub> _hubcontext,
            IKakaoService _kakaoservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.VocInfoRepository = _vocinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.AlarmInfoRepository = _alarminforepository;
            this.UserInfoRepository = _userinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;

            this.HubContext = _hubcontext;
            this.KakaoService = _kakaoservice;

            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocListDTO?>> GetVocList(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<VocListDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<VocListDTO>? model = await VocInfoRepository.GetVocList(Convert.ToInt32(placeid));

                if (model is [_, ..])
                    return new ResponseList<VocListDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장의 선택된 일자의 VOC LIST 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocListDTO>?> GetVocFilterList(HttpContext? context, DateTime? startdate, DateTime? enddate, int? type, int? status,int? buildingid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (startdate is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (enddate is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (type is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (status is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (buildingid is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };


                List<VocListDTO>? model = await VocInfoRepository.GetVocFilterList(Convert.ToInt32(PlaceIdx), startdate, enddate, type, status, buildingid);
                if (model is [_, ..])
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTO>(), code = 500 };
            }
        }




        /// <summary>
        /// VOC 상세보기 - 직원용
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<VocEmployeeDetailDTO?>> GetVocDetail(HttpContext? context, int? vocid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (vocid is null)
                    return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? model = await VocInfoRepository.GetVocInfoById(vocid);
                if(model is not null)
                {
                    BuildingTb? building = await BuildingInfoRepository.GetBuildingInfo(model.BuildingTbId);
                    if(building is not null)
                    {
                        VocEmployeeDetailDTO dto = new VocEmployeeDetailDTO();
                        dto.Id = model.Id; // 민원 인덱스
                        dto.Code = model.Code; // 접수코드
                        dto.CreateDT = model.CreateDt; // 민원 신청일
                        dto.Status = model.Status; // 민원상태
                        dto.BuildingName = building.Name; // 건물명
                        dto.Type = model.Type; // 민원유형
                        dto.Title = model.Title; // 민원제목
                        dto.Contents = model.Content; // 민원내용
                        dto.CreateUser = model.CreateUser; // 민원인
                        dto.Phone = model.Phone; // 민원인 전화번호

                        string VocFileName = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, PlaceIdx);
                        if (!String.IsNullOrWhiteSpace(model.Image1))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, model.Image1);
                            dto.Images.Add(ImageBytes);
                        }
                        if (!String.IsNullOrWhiteSpace(model.Image2))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, model.Image2);
                            dto.Images.Add(ImageBytes);
                        }
                        if (!String.IsNullOrWhiteSpace(model.Image3))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(VocFileName, model.Image3);
                            dto.Images.Add(ImageBytes);
                        }

                        return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocEmployeeDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// voc 유형 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateVocTypeService(HttpContext? context, UpdateVocDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                if(VocTB is not null)
                {
                    VocTB.Type = dto.Type;
                    VocTB.UpdateDt = DateTime.Now;
                    VocTB.UpdateUser = creater;

                    bool UpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB);
                    if(UpdateResult)
                    {
                        BuildingTb? BuildingTB;
                        List<UsersTb>? Users;
                        switch (dto.Type)
                        {
#region 기타민원 타입변경
                            case 0:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if(VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocDefaultList(BuildingTB!.PlaceTbId);
                                if (Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_ETCRoom").SendAsync("ReceiveVoc", "[기타] 민원 등록되었습니다");
                                }

                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 기계민원 타입변경
                            case 1:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if(VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if (BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocMachineList(BuildingTB!.PlaceTbId);

                                if (Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_MCRoom").SendAsync("ReceiveVoc", "[기계] 민원 등록되었습니다");
                                }
                                
                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 전기민원 타입변경
                            case 2:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocElecList(BuildingTB!.PlaceTbId);

                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_ELECRoom").SendAsync("ReceiveVoc", "[전기] 민원 등록되었습니다");
                                }

                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion
#region 승강민원 타입변경
                            case 3:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocLiftList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_LFRoom").SendAsync("ReceiveVoc", "[승강] 민원 등록되었습니다");
                                }
                                
                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 소방민원 타입변경
                            case 4:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocFireList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_FRRoom").SendAsync("ReceiveVoc", "[소방] 민원 등록되었습니다");
                                }

                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 건축민원 타입변경
                            case 5:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if (BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocConstructList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);

                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_CSTRoom").SendAsync("ReceiveVoc", "[건축] 민원 등록되었습니다");
                                }
                                
                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 통신민원 타입변경
                            // 통신
                            case 6:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocNetWorkList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);

                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_NTRoom").SendAsync("ReceiveVoc", "[통신] 민원 등록되었습니다");
                                }

                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 미화민원 타입변경
                            // 미화
                            case 7:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if(BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocBeautyList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);
                                    
                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_BEAUTYRoom").SendAsync("ReceiveVoc", "[미화] 민원 등록되었습니다");
                                }
                                
                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion

#region 보안민원 타입변경
                            // 보안
                            case 8:
                                VocTB = await VocInfoRepository.GetVocInfoById(dto.VocID);
                                if (VocTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB!.BuildingTbId);
                                if (BuildingTB is null)
                                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                                Users = await UserInfoRepository.GetVocSecurityList(BuildingTB!.PlaceTbId);
                                if(Users is [_, ..])
                                {
                                    // 여기에 해당하는 관리자들에다 Alarm 테이블 INSERT
                                    bool AddAlarm = await SetMessage(Users, VocTB.CreateUser!, dto.VocID);

                                    // 소켓전송
                                    await HubContext.Clients.Group($"{placeidx}_SECURoom").SendAsync("ReceiveVoc", "[보안] 민원 등록되었습니다");
                                }

                                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
#endregion
                        }

                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "조회결과가 존재하지 않습니다.", data = null, code = 404 };
                }


            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// VOC 알람메시지 처리
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="VocName"></param>
        /// <param name="VocTableIdx"></param>
        /// <returns></returns>
        public async ValueTask<bool> SetMessage(List<UsersTb>? userlist, string Creater, int VocTableIdx)
        {
            try
            {
                if (userlist is [_, ..])
                {
                    foreach (var user in userlist)
                    {
                        // 사용자 수만큼 Alarm 테이블에 Insert
                        AlarmTb alarm = new AlarmTb()
                        {
                            CreateDt = DateTime.Now,
                            CreateUser = Creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = Creater,
                            UsersTbId = user.Id,
                            VocTbId = VocTableIdx
                        };

                        AlarmTb? alarm_result = await AlarmInfoRepository.AddAsync(alarm);
                        if (alarm_result is null)
                            return false;
                    }
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return false;
            }
        }


    }
}
