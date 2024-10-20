using FamTec.Server.Repository.UseMaintenence;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;

namespace FamTec.Server.Services.UseMaintenence
{
    public class UseMaintenenceService : IUseMaintenenceService
    {
        private readonly IUseMaintenenceInfoRepository UseMaintenenceInfoRepository;
        private readonly ILogService LogService;
        private readonly ILogger<UseMaintenenceService> BuilderLogger;

        public UseMaintenenceService(IUseMaintenenceInfoRepository _usemaintenenceinforepository,
            ILogService _logservice,
            ILogger<UseMaintenenceService> _builderlogger)
        {
            this.UseMaintenenceInfoRepository = _usemaintenenceinforepository;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        /// <summary>
        /// ASP - 빌드로그
        /// </summary>
        /// <param name="ex"></param>
        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 사용자재 상세 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<UseMaterialDetailDTO>> GetDetailUseMaterialService(HttpContext context, int usematerialid, int materialid, int roomid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(usematerialid > 0)
                {
                    // 수정건
                    UseMaterialDetailDTO? model = await UseMaintenenceInfoRepository.GetDetailUseStoreList(usematerialid, Int32.Parse(placeid)).ConfigureAwait(false);
                    if (model is not null)
                        return new ResponseUnit<UseMaterialDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                    else
                        return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
                else
                {
                    // 신규건
                    UseMaterialDetailDTO? model = await UseMaintenenceInfoRepository.New_GetDetailUseStoreList(materialid, roomid, Int32.Parse(placeid)).ConfigureAwait(false);
                    if (model is not null)
                        return new ResponseUnit<UseMaterialDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                    else
                        return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<UseMaterialDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자재 수정 서비스 - 추가출고 / 입고처리
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<bool?>> UpdateDetailUseMaterialService(HttpContext context, UpdateMaintenanceMaterialDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                

                UseMaintenenceMaterialTb? UseMaterialTB = await UseMaintenenceInfoRepository.GetUseMaintanceInfo(dto.UseMaintanceID, Int32.Parse(placeid));
                if(UseMaterialTB is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                // 현재 유지보수건에 대해서 해당창고의 해당품목에 해당하는게 몇개가 출고됐는지 조회 - 입고 / 출고 구분로직
                //int? ThisUseNum = await UseMaintenenceInfoRepository.UseThisMaterialNum(Convert.ToInt32(placeid), UseMaterialTB.MaintenanceTbId, UseMaterialTB.RoomTbId, UseMaterialTB.MaterialTbId).ConfigureAwait(false);


                if (dto.Num > UseMaterialTB.Num)
                {
                    // 출고
                    // 출고 일떄는 가용 수량을 봐야함.
                    int? UseAvailableNum = await UseMaintenenceInfoRepository.UseAvailableMaterialNum(Convert.ToInt32(placeid), UseMaterialTB.RoomTbId, UseMaterialTB.MaterialTbId).ConfigureAwait(false);
                    

                    if (UseAvailableNum is null)
                        return new ResponseUnit<bool?>() { message = "품목의 개수가 부족합니다.", data = false, code = 204 };


                    if (UseAvailableNum >= (dto.Num - UseMaterialTB.Num))
                    {
                        // 가능
                        int? UpdateResult = await UseMaintenenceInfoRepository.UseMaintanceOutput(Int32.Parse(placeid), updater, dto).ConfigureAwait(false);
                        if (UpdateResult > 0)
                        {
                            return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else if (UpdateResult == -1)
                        {
                            return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 201 };
                        }
                        else if (UpdateResult == -2)
                        {
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                        }
                        else
                        {
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }
                    }
                    else
                    {
                        // 가용수량보다 부족해서 안됨
                        return new ResponseUnit<bool?>() { message = "품목의 개수가 부족합니다.", data = false, code = 204 };
                    }
                }
                else if (dto.Num < UseMaterialTB.Num)
                {
                    // 입고
                    int? UpdateResult = await UseMaintenenceInfoRepository.UseMatintanceInput(Int32.Parse(placeid), updater, dto).ConfigureAwait(false);
                    if (UpdateResult > 0)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else if (UpdateResult == -1)
                    {
                        return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 201 };
                    }
                    else if (UpdateResult == -2)
                    {
                        return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    // 아무것도 아님
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> UpdateUseMaintanceService(HttpContext context, UpdateMaintancematerialDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int result = await UseMaintenenceInfoRepository.UpdateUseMaintance(dto, Convert.ToInt32(placeid), updater).ConfigureAwait(false);
                if (result == 1)
                    return new ResponseUnit<bool?>() { message = "요청이 정상처리되었습니다.", data = true, code = 200 };
                else if (result == -1)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                else if (result == -2)
                    return new ResponseUnit<bool?>() { message = "수량이 부족합니다.", data = false, code = 204 };
                else if (result == -3)
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 401 };
                else
                    return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
