using FamTec.Server.Helpers;
using FamTec.Server.Hubs;
using FamTec.Server.Repository.UseMaintenence;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.AspNetCore.SignalR;

namespace FamTec.Server.Services.UseMaintenence
{
    public class UseMaintenenceService : IUseMaintenenceService
    {
        private readonly IUseMaintenenceInfoRepository UseMaintenenceInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        IHubContext<BroadcastHub> HubContext; /* SignalR 허브 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<UseMaintenenceService> CreateBuilderLogger; /* 콘솔로그 */

        public UseMaintenenceService(IUseMaintenenceInfoRepository _usemaintenenceinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            IHubContext<BroadcastHub> _hubcontext,
            ILogService _logservice,
            ConsoleLogService<UseMaintenenceService> _createbuilderlogger)
        {
            this.UseMaintenenceInfoRepository = _usemaintenenceinforepository;
            this.LogService = _logservice;
            this.HubContext = _hubcontext;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 사용자재 상세 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseModel<UseMaterialDetailDTO>> GetDetailUseMaterialService(int usematerialid, int materialid, int roomid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(usematerialid > 0)
                {
                    // 수정건
                    UseMaterialDetailDTO? model = await UseMaintenenceInfoRepository.GetDetailUseStoreList(usematerialid, Int32.Parse(placeid)).ConfigureAwait(false);
                    if (model is not null)
                        return new ResponseModel<UseMaterialDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                    else
                        return new ResponseModel<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
                else
                {
                    // 신규건
                    UseMaterialDetailDTO? model = await UseMaintenenceInfoRepository.New_GetDetailUseStoreList(materialid, roomid, Int32.Parse(placeid)).ConfigureAwait(false);
                    if (model is not null)
                        return new ResponseModel<UseMaterialDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                    else
                        return new ResponseModel<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<UseMaterialDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자재 수정 서비스 - 추가출고 / 입고처리
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> UpdateDetailUseMaterialService(UpdateMaintenanceMaterialDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(updater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UseMaintenenceMaterialTb? UseMaterialTB = await UseMaintenenceInfoRepository.GetUseMaintanceInfo(dto.UseMaintanceID, Int32.Parse(placeid)).ConfigureAwait(false);
                if(UseMaterialTB is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if (dto.Num > UseMaterialTB.Num)
                {
                    // 출고
                    // 출고 일떄는 가용 수량을 봐야함.
                    int? UseAvailableNum = await UseMaintenenceInfoRepository.UseAvailableMaterialNum(Convert.ToInt32(placeid), UseMaterialTB.RoomTbId, UseMaterialTB.MaterialTbId).ConfigureAwait(false);
                    

                    if (UseAvailableNum is null)
                        return new ResponseModel<bool?>() { message = "품목의 개수가 부족합니다.", data = false, code = 204 };


                    if (UseAvailableNum >= (dto.Num - UseMaterialTB.Num))
                    {
                        // 가능
                        int? UpdateResult = await UseMaintenenceInfoRepository.UseMaintanceOutput(Int32.Parse(placeid), updater, dto).ConfigureAwait(false);
                        if (UpdateResult > 0)
                        {
                            // 자재 상태 알림
                            await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                            
                            // 유지보수 상태 알림
                            await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                            return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else if (UpdateResult == -1)
                        {
                            return new ResponseModel<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 201 };
                        }
                        else if (UpdateResult == -2)
                        {
                            return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                        }
                        else
                        {
                            return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }
                    }
                    else
                    {
                        // 가용수량보다 부족해서 안됨
                        return new ResponseModel<bool?>() { message = "품목의 개수가 부족합니다.", data = false, code = 204 };
                    }
                }
                else if (dto.Num < UseMaterialTB.Num)
                {
                    // 입고
                    int? UpdateResult = await UseMaintenenceInfoRepository.UseMatintanceInput(Int32.Parse(placeid), updater, dto).ConfigureAwait(false);
                    if (UpdateResult > 0)
                    {
                        // 자재 상태 알림
                        await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);
                        
                        // 유지보수 상태 알림
                        await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                        return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else if (UpdateResult == -1)
                    {
                        return new ResponseModel<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 201 };
                    }
                    else if (UpdateResult == -2)
                    {
                        return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    // 아무것도 아님
                    return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
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

        public async Task<ResponseModel<bool?>> UpdateUseMaintanceService(UpdateMaintancematerialDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(updater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int result = await UseMaintenenceInfoRepository.UpdateUseMaintance(dto, Convert.ToInt32(placeid), updater).ConfigureAwait(false);
                if (result == 1)
                {
                    // 자재 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaterialStatus").SendAsync("ReceiveMaterialStatus", "자재의 상태가 변경되었습니다.").ConfigureAwait(false);

                    // 유지보수 상태 알림
                    await HubContext.Clients.Group($"{placeid}_MaintenanceStatus").SendAsync("ReceiveMaintenanceStatusStatus", "유지보수 상태가 변경되었습니다.").ConfigureAwait(false);

                    return new ResponseModel<bool?>() { message = "요청이 정상처리되었습니다.", data = true, code = 200 };
                }
                else if (result == -1)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                else if (result == -2)
                    return new ResponseModel<bool?>() { message = "수량이 부족합니다.", data = false, code = 204 };
                else if (result == -3)
                    return new ResponseModel<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 401 };
                else
                    return new ResponseModel<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
