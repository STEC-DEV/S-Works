using FamTec.Server.Helpers;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Maintenence;
using FamTec.Server.Repository.User;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.SignalR;

namespace FamTec.Server.Services.Maintenance
{
    public class MaintanceService : IMaintanceService
    {
        private readonly IMaintanceRepository MaintanceRepository;
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<MaintanceService> CreateBuilderLogger; /* 콘솔로그 */
        private DirectoryInfo? di; /* 디렉터리 객체 */
        private string? MaintanceFileFolderPath; /* 디렉터리 경로 */
        IHubContext<BroadcastHub> HubContext; /* SignalR 허브 */

        public MaintanceService(IMaintanceRepository _maintancerepository,
            IFacilityInfoRepository _facilityinforepository,
            IUserInfoRepository _userinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            IHubContext<BroadcastHub> _hubcontext,
            IFileService _fileservice,
            ILogService _logservice,
            ConsoleLogService<MaintanceService> _createbuilderlogger)
        {
            this.MaintanceRepository = _maintancerepository;
            this.FacilityInfoRepository = _facilityinforepository;
            this.UserInfoRepository = _userinforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.HubContext = _hubcontext;
            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 대쉬보드용 금일 유지보수 이력 리스트
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MaintenanceDaysDTO>>?> GetMaintenanceDaysList()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintenanceDaysDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseModel<List<MaintenanceDaysDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime NowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                List<MaintenanceDaysDTO>? model = await MaintanceRepository.GetMaintenanceDaysData(NowDate, Convert.ToInt32(placeidx)).ConfigureAwait(false);


                return new ResponseModel<List<MaintenanceDaysDTO>>() { message = "OK", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintenanceDaysDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 대쉬보드용 1년치 유지보수비용 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MaintanceYearPriceDTO>>?> GetMaintenanceYearPriceList()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 현재 년도의 1월 1일
                DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);

                // 현재년도의 마지막 12년도의 일의 11시59분59초
                DateTime lastDayOfDecember = new DateTime(DateTime.Now.Year, 12, DateTime.DaysInMonth(DateTime.Now.Year, 12), 23, 59, 59);

                List<MaintanceYearPriceDTO>? model = await MaintanceRepository.GetMaintenanceYearData(firstDayOfYear, lastDayOfDecember, Convert.ToInt32(placeid)).ConfigureAwait(false);

                if (model is [_, ..])
                    return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else if(model.Count == 0)
                    return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "요청이 정상 처리되었습니다.", data = model, code = 204 };
                else
                    return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseModel<List<MaintanceWeekCount>>?> GetMaintanceDashBoardDataService()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintanceWeekCount>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<MaintanceWeekCount>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ToDays = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                DateTime startOfWeek = ToDays.AddDays(-7);

                DateTime EndOfWeek = ToDays.AddDays(1).AddTicks(-1);

                List<MaintanceWeekCount>? WeekListData = await MaintanceRepository.GetMaintanceDashBoardData(startOfWeek, EndOfWeek, Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (WeekListData is null)
                {
                    ResponseModel<List<MaintanceWeekCount>> model = new ResponseModel<List<MaintanceWeekCount>>()
                    {
                        message = "요청이 정상처리되었습니다.",
                        data = new List<MaintanceWeekCount>(),
                        code = 204
                    };

                    return model;
                }
                else
                {
                    ResponseModel<List<MaintanceWeekCount>> model = new ResponseModel<List<MaintanceWeekCount>>()
                    {
                        message = "요청이 정상처리되었습니다.",
                        data = WeekListData,
                        code = 200
                    };

                    return model;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintanceWeekCount>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseModel<bool?>> AddMaintanceImageService(int id, IFormFile? files)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? ImageAddResult = await MaintanceRepository.AddMaintanceImageAsync(id, Int32.Parse(placeid), files).ConfigureAwait(false);
                return ImageAddResult switch
                {
                    true => new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 출고등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<FailResult?>> AddMaintanceService(AddMaintenanceDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);
                string? userid = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(userid))
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto.Type is 0) // 자체작업
                {
                    FailResult? MaintanceId = await MaintanceRepository.AddSelfMaintanceAsync(dto, creater, userid, Convert.ToInt32(placeid)).ConfigureAwait(false);

                    if (MaintanceId!.ReturnResult > 0)
                    {
                        // 자재 상태 알림
                        await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                        // 유지보수 상태 알림
                        await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                        return new ResponseModel<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = MaintanceId, code = 200 };
                    }
                    else if (MaintanceId!.ReturnResult == 0)
                    {
                        return new ResponseModel<FailResult?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = MaintanceId, code = 422 };
                    }
                    else if (MaintanceId!.ReturnResult == -1)
                    {
                        return new ResponseModel<FailResult?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = MaintanceId, code = 409 };
                    }
                    else if (MaintanceId!.ReturnResult == -2)
                    {
                        return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = MaintanceId, code = 404 };
                    }
                    else
                    {
                        return new ResponseModel<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = MaintanceId, code = 500 };
                    }
                }
                else if (dto.Type is 1) // 외주작업
                {
                    if (dto.TotalPrice is 0)
                        return new ResponseModel<FailResult?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                    FailResult? MaintanceId = await MaintanceRepository.AddOutSourcingMaintanceAsync(dto, creater, userid, Convert.ToInt32(placeid)).ConfigureAwait(false);

                    if (MaintanceId!.ReturnResult > 0)
                    {
                        // 유지보수 상태 알림 - 외주작업은 자재를 안쓰기때문에 소켓 1개
                        await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                        return new ResponseModel<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = MaintanceId, code = 200 };
                    }
                    else if(MaintanceId!.ReturnResult == -2)
                    {
                        return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = MaintanceId, code = 404 };
                    }
                    else
                    {
                        return new ResponseModel<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = MaintanceId, code = 500 };
                    }
                }
                else
                {
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자재 추가출고
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<FailResult?>> AddSupMaintanceService(AddMaintanceMaterialDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);
                string? userid = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(userid))
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                FailResult? MaintanceId = await MaintanceRepository.AddMaintanceMaterialAsync(dto, creater, Convert.ToInt32(placeid)).ConfigureAwait(false);
                if(MaintanceId!.ReturnResult > 0)
                {
                    // 자재 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                    // 유지보수 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                    return new ResponseModel<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = MaintanceId, code = 200 };
                }
                else if(MaintanceId!.ReturnResult == 0)
                {
                    return new ResponseModel<FailResult?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = MaintanceId, code = 422 };
                }
                else if(MaintanceId!.ReturnResult == -1)
                {
                    return new ResponseModel<FailResult?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = MaintanceId, code = 409 };
                }
                else if(MaintanceId!.ReturnResult == -2)
                {
                    return new ResponseModel<FailResult?>() { message = "잘못된 요청입니다.", data = MaintanceId, code = 404 };
                }
                else
                {
                    return new ResponseModel<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = MaintanceId, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 유지보수의 출고내역 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> DeleteMaintenanceStoreRecordService(DeleteMaintanceDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? deleter = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(deleter))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                bool? DeleteResult = await MaintanceRepository.deleteMaintenanceStoreRecord(dto, Int32.Parse(placeid), deleter).ConfigureAwait(false);
                if(DeleteResult == true)
                {
                    // 자재 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                    
                    // 유지보수 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                    return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    return new ResponseModel<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 200 };
                }
                else
                {
                    return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> DeleteMaintenanceRecordService(DeleteMaintanceDTO2 dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? deleter = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(deleter))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                bool? DeleteResult = await MaintanceRepository.deleteMaintenanceRecord(dto, Int32.Parse(placeid), deleter).ConfigureAwait(false);

                if(DeleteResult == true)
                {
                    // 자재 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                    
                    // 유지보수 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                    return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    return new ResponseModel<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MaintanceListDTO>>> GetMaintanceHistoryService(int facilityid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintanceListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<MaintanceListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                // 여기 더 추가해야함
                
                FacilityTb? VaildFacility = await FacilityInfoRepository.GetFacilityInfo(facilityid).ConfigureAwait(false);
                if(VaildFacility is null)
                    return new ResponseModel<List<MaintanceListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceListDTO>? dto = await MaintanceRepository.GetFacilityHistoryList(facilityid, Int32.Parse(placeid)).ConfigureAwait(false);
                

                if (dto is not null && dto.Any())
                    return new ResponseModel<List<MaintanceListDTO>>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseModel<List<MaintanceListDTO>>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintanceListDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MaintanceHistoryDTO>>?> GetDateHistoryList(DateTime StartDate, DateTime EndDate, List<string> category, List<int> type)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserId = Convert.ToString(context.Items["UserIdx"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                
                if(String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserId))
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserId));
                if(UserTB is null)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // -- USER 권한검사
                // 권한과 명칭을 쌍으로 매핑
                var permMapping = new Dictionary<Func<bool>, string>
                {
                    { () => UserTB.PermBasic > 0, "기타" },
                    { () => UserTB.PermMachine > 0, "기계" },
                    { () => UserTB.PermElec > 0, "전기" },
                    { () => UserTB.PermLift > 0, "승강" },
                    { () => UserTB.PermFire > 0, "소방" },
                    { () => UserTB.PermConstruct > 0, "건축" },
                    { () => UserTB.PermNetwork > 0, "통신" },
                    { () => UserTB.PermBeauty > 0, "미화" },
                    { () => UserTB.PermSecurity > 0, "보안" }
                };

                // 권한이 있는 항목만 리스트에 추가
                List<string> UserPerm = permMapping.Where(p => p.Key()).Select(p => p.Value).ToList();

                // category와 비교하여 없는 항목 찾기
                List<string> PermCheck = category.Except(UserPerm).ToList();

                if (PermCheck.Count() > 0)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "권한이 일치하지 않습니다.", data = null, code = 401 };

                List<MaintanceHistoryDTO>? model = await MaintanceRepository.GetDateHistoryList(Convert.ToInt32(placeid), StartDate, EndDate, category, type).ConfigureAwait(false);

                if (model is not null && model.Any())
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 속한 사업장 유지보수 월별 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchdate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MaintanceHistoryDTO>>?> GetMonthHistoryList(string searchdate, List<string> category, List<int> type)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserId = Convert.ToString(context.Items["UserIdx"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserId))
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserId));
                if (UserTB is null)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // -- USER 권한검사
                // 권한과 명칭을 쌍으로 매핑
                var permMapping = new Dictionary<Func<bool>, string>
                {
                    { () => UserTB.PermBasic > 0, "기타" },
                    { () => UserTB.PermMachine > 0, "기계" },
                    { () => UserTB.PermElec > 0, "전기" },
                    { () => UserTB.PermLift > 0, "승강" },
                    { () => UserTB.PermFire > 0, "소방" },
                    { () => UserTB.PermConstruct > 0, "건축" },
                    { () => UserTB.PermNetwork > 0, "통신" },
                    { () => UserTB.PermBeauty > 0, "미화" },
                    { () => UserTB.PermSecurity > 0, "보안" }
                };

                // 권한이 있는 항목만 리스트에 추가
                List<string> UserPerm = permMapping.Where(p => p.Key()).Select(p => p.Value).ToList();

                // category와 비교하여 없는 항목 찾기
                List<string> PermCheck = category.Except(UserPerm).ToList();

                if (PermCheck.Count() > 0)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "권한이 일치하지 않습니다.", data = null, code = 401 };


                string[] splitDate = searchdate.Split('-');
                string year = splitDate[0];
                string month = splitDate[1].PadLeft(2, '0'); // 한 자리 월을 두 자리로 맞추기 위해 앞에 0 추가

                int checkResult;

                // 년도 숫자값 맞는지 검사
                bool checkDate = int.TryParse(year, out checkResult);
                if (!checkDate)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 월 숫자값 맞는지 검사
                checkDate = int.TryParse(month, out checkResult);
                if(!checkDate)
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceHistoryDTO>? model = await MaintanceRepository.GetMonthHistoryList(Convert.ToInt32(placeid), category, type, Convert.ToInt32(year), Convert.ToInt32(month)).ConfigureAwait(false);

                if (model is not null && model.Any())
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<MaintanceHistoryDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<AllMaintanceHistoryDTO>>?> GetAllHistoryList(List<string> category, List<int> type)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserId = Convert.ToString(context.Items["UserIdx"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserId))
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserId));
                if (UserTB is null)
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // -- USER 권한검사
                // 권한과 명칭을 쌍으로 매핑
                var permMapping = new Dictionary<Func<bool>, string>
                {
                    { () => UserTB.PermBasic > 0, "기타" },
                    { () => UserTB.PermMachine > 0, "기계" },
                    { () => UserTB.PermElec > 0, "전기" },
                    { () => UserTB.PermLift > 0, "승강" },
                    { () => UserTB.PermFire > 0, "소방" },
                    { () => UserTB.PermConstruct > 0, "건축" },
                    { () => UserTB.PermNetwork > 0, "통신" },
                    { () => UserTB.PermBeauty > 0, "미화" },
                    { () => UserTB.PermSecurity > 0, "보안" }
                };

                // 권한이 있는 항목만 리스트에 추가
                List<string> UserPerm = permMapping.Where(p => p.Key()).Select(p => p.Value).ToList();

                // category와 비교하여 없는 항목 찾기
                List<string> PermCheck = category.Except(UserPerm).ToList();

                if (PermCheck.Count() > 0)
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "권한이 일치하지 않습니다.", data = null, code = 401 };


                List<AllMaintanceHistoryDTO>? model = await MaintanceRepository.GetAllHistoryList(Convert.ToInt32(placeid), category, type).ConfigureAwait(false);

                if (model is not null && model.Any())
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<AllMaintanceHistoryDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 설비의 유지보수리스트중 하나 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaintanceID"></param>
        /// <returns></returns>
        public async Task<ResponseModel<DetailMaintanceDTO?>> GetDetailService(int MaintanceID, bool isMobile)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 수정임
                DetailMaintanceDTO? model = await MaintanceRepository.DetailMaintanceList(MaintanceID, Int32.Parse(placeid), isMobile).ConfigureAwait(false);

                if (model is not null)
                    return new ResponseModel<DetailMaintanceDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseModel<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<DetailMaintanceDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> UpdateMaintenanceService(UpdateMaintenanceDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;

                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(UserIdx) || String.IsNullOrWhiteSpace(updater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                MaintanceFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Maintance");

                di = new DirectoryInfo(MaintanceFileFolderPath);
                if (!di.Exists) di.Create();

                MaintenenceHistoryTb? model = await MaintanceRepository.GetMaintenanceInfo(dto.MaintanceID).ConfigureAwait(false);

                if(model is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Name = dto.WorkName!; // 유지보수명
                model.Worker = dto.Worker!; // 작업자
                model.UpdateDt = ThisDate;
                model.UpdateUser = updater;

                if(files is not null) // 파일이 공백이 아닌 경우
                {
                    if(files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다르면
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            deleteFileName = model.Image;
                        }

                        // 새로운 파일명 설정
                        string newFileName = FileService.SetNewFileName(UserIdx, files);
                        NewFileName = newFileName; // 파일명 리스트에 추가
                        model.Image = newFileName; // DB Image명칭 업데이트

                        RemoveTemp = newFileName; // 실패시 삭제명단에 넣어야함.
                    }
                }
                else // 파일이 공백인 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB의 이미지가 공백이 아니면
                    {
                        deleteFileName = model.Image; // 기존 파일 삭제 목록에 추가
                        model.Image = null; // 모델의 파일명 비우기
                    }
                }

                // 먼저 파일 삭제 처리
                // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                byte[]? ImageBytes = null;
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    ImageBytes = await FileService.GetImageFile(MaintanceFileFolderPath, deleteFileName).ConfigureAwait(false);
                }

                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if(ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(MaintanceFileFolderPath, deleteFileName);
                }

                // 새 파일 저장
                if(files is not null)
                {
                    if(String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, MaintanceFileFolderPath, files).ConfigureAwait(false);
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? updateMaintance = await MaintanceRepository.UpdateMaintenanceInfo(model).ConfigureAwait(false);
                if(updateMaintance == true)
                {
                    // 유지보수 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                    // 성공하면 그걸로 끝
                    return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을 원래대로 돌려놔야함.
                    if (AddTemp is not null)
                    {
                        try
                        {
                            if(FileService.IsFileExists(MaintanceFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, MaintanceFileFolderPath, files).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(MaintanceFileFolderPath, RemoveTemp);
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}