﻿using FamTec.Server.Hubs;
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
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.SignalR;

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
        
        private readonly ConsoleLogService<VocService> CreateBuilderLogger;

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
            ILogService _logservice,
            ConsoleLogService<VocService> _createbuilderlogger)
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
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <param name="context"></param>
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
        /// <param name="context"></param>
        /// <param name="date"></param>
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
        /// 월간 사업장 VOC 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <param name="division"></param>
        /// <param name="searchDate"></param>
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
        /// 기간 사업장 VOC 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <param name="division"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

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
        /// <param name="context"></param>
        /// <param name="vocid"></param>
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
        /// <param name="context"></param>
        /// <param name="dto"></param>
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
        /// <param name="userlist"></param>
        /// <param name="VocName"></param>
        /// <param name="VocTableIdx"></param>
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
        /// <param name="context"></param>
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
        /// <param name="context"></param>
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
        /// <param name="conteext"></param>
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
        /// <param name="context"></param>
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
