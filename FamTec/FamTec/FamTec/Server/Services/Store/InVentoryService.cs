using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Store;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.JSInterop.Infrastructure;
using System.Reflection.Metadata.Ecma335;

namespace FamTec.Server.Services.Store
{
    public class InVentoryService : IInVentoryService
    {
        private readonly IInventoryInfoRepository InventoryInfoRepository;
        private readonly IStoreInfoRepository StoreInfoRepository;

        private readonly ILogService LogService;
        private readonly ConsoleLogService<InVentoryService> CreateBuilderLogger;

        public InVentoryService(IInventoryInfoRepository _inventoryinforepository,
            IStoreInfoRepository _storeinforepository,
            ILogService _logservice,
            ConsoleLogService<InVentoryService> _createbuilderlogger)
        {
            this.InventoryInfoRepository = _inventoryinforepository;
            this.StoreInfoRepository = _storeinforepository;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 입고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> AddInStoreService(HttpContext context, List<InOutInventoryDTO> dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = 0, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                
                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = 0, code = 404 };

                // 인벤토리 테이블에 ADD
                int? AddInStore = await InventoryInfoRepository.AddAsync(dto, creater, Convert.ToInt32(placeid)).ConfigureAwait(false);
                return AddInStore switch
                {
                    1 => new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = 1, code = 200 },
                    -1 => new ResponseUnit<int?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = -1, code = 200 },
                    _ => new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 출고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<FailResult?>> OutInventoryService(HttpContext context, List<InOutInventoryDTO> dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                FailResult? OutResult = await InventoryInfoRepository.SetOutInventoryInfo(dto, creater, Convert.ToInt32(placeid)).ConfigureAwait(false);
                /*
                    0 : 출고수량이 부족함
                    1 : 출고성공
                    -1 : 트랜잭션 걸림
                    -2 : 이미 삭제된 정보에 접근하고자 함.
                 */

                return OutResult!.ReturnResult switch
                {
                    1 => new ResponseUnit<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = OutResult, code = 200 },
                    0 => new ResponseUnit<FailResult?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = OutResult, code = 422 },
                    -1 => new ResponseUnit<FailResult?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = OutResult, code = 409 },
                    -2 => new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = OutResult, code = 404 },
                    _ => new ResponseUnit<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = OutResult, code = 500 }
                };
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장의 입-출고 이력 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> GetPlaceInOutCountService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int count = await StoreInfoRepository.GetPlaceInOutCount(Int32.Parse(placeid)).ConfigureAwait(false);
                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 입-출고 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<InOutHistoryListDTO>> GetInOutHistoryService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutHistoryListDTO>? model = await StoreInfoRepository.GetInOutList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (model is not null && model.Any())
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<InOutHistoryListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 입-출고 이력 페이지네이션 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<ResponseList<InOutHistoryListDTO>> GetInoutPageNationHistoryService(HttpContext context, int pagenum, int pagesize)
        {
            try
            {
                if (context is null)
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutHistoryListDTO>? model = await StoreInfoRepository.GetInOutPageNationList(Convert.ToInt32(placeid), pagenum, pagesize).ConfigureAwait(false);

                if (model is not null && model.Any())
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<InOutHistoryListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 품목별 기간별 입출고 이력조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ResponseList<PeriodicDTO>> PeriodicInventoryRecordService(HttpContext context, List<int> materialid, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PeriodicDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
           
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<PeriodicDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<PeriodicDTO>? model = await InventoryInfoRepository.GetInventoryRecord(Convert.ToInt32(placeid), materialid, startDate, endDate).ConfigureAwait(false);
                
                // ** 주석처리
                if(model is [_, ..])
                {
                    foreach(PeriodicDTO dto in model)
                    {
                        // 총 입고 수량
                        dto.TotalInputNum = dto.InventoryList.Where(m => m.Type == 1 && m.InOutNum.HasValue)
                                    .Sum(m => m.InOutNum ?? 0);

                        // 총 입고 단가
                        dto.TotalInputUnitPrice = dto.InventoryList.Where(m => m.Type == 1 && m.InOutUnitPrice.HasValue)
                            .Sum(m => m.InOutUnitPrice ?? 0);

                        // 총 입고 금액
                        dto.TotalInputPrice = dto.InventoryList.Where(m => m.Type == 1 && m.InOutTotalPrice.HasValue)
                            .Sum(m => m.InOutTotalPrice ?? 0);

                        // 총 출고 수량
                        dto.TotalOutputNum = dto.InventoryList.Where(m => m.Type == 0 && m.InOutNum.HasValue)
                            .Sum(m => m.InOutNum ?? 0);

                        // 총 출고 단가
                        dto.TotalOutputUnitPrice = dto.InventoryList.Where(m => m.Type == 0 && m.InOutUnitPrice.HasValue)
                            .Sum(m => m.InOutUnitPrice ?? 0);

                        // 총 출고 금액
                        dto.TotalOutputPrice = dto.InventoryList.Where(m => m.Type == 0 && m.InOutTotalPrice.HasValue)
                            .Sum(m => m.InOutTotalPrice ?? 0);

                        // 총 재고수량
                        //dto.TotalStockNum = dto.InventoryList.Sum(m => m.CurrentNum ?? 0);
                        // 마지막 재고수량으로 변경됨
                        dto.TotalStockNum = dto.InventoryList.OrderBy(m => m.INOUT_DATE).LastOrDefault()?.CurrentNum ?? 0;

                        // 이월재고 수량
                        dto.LastMonthStock = await InventoryInfoRepository.GetCarryOverNum(Convert.ToInt32(placeid), dto.ID!.Value, startDate);
                    }
                }
                
                if (model is [_, ..])
                    return new ResponseList<PeriodicDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<PeriodicDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<PeriodicDTO>() { message = "서버에서 요청을 처리하지 못하였습니다", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장별 재고 현황 - 공간 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResponseList<MaterialHistory>> GetPlaceInventoryRecordService(HttpContext context, List<int> materialid, bool type)
        {
            try
            {
                if(context is null || materialid is null)
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaterialHistory>? model = await InventoryInfoRepository.GetPlaceInventoryRecord(Convert.ToInt32(placeid), materialid, type).ConfigureAwait(false);
                
                if(model is null)
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                else if(model.Count > 0)
                    return new ResponseList<MaterialHistory>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<MaterialHistory>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<MaterialHistory>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 품목의 재고수량 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaterialId"></param>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<InOutLocationDTO>> GetMaterialRoomInventoryNumService(HttpContext context, int MaterialId, int RoomId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                InOutLocationDTO? model = await InventoryInfoRepository.GetLocationMaterialInventoryInfo(Convert.ToInt32(placeid), MaterialId, RoomId);
                if (model is not null)
                    return new ResponseUnit<InOutLocationDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseUnit<InOutLocationDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<InOutLocationDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 품목의 재고수량 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public async Task<ResponseList<InOutLocationDTO>> GetMaterialRoomNumService(HttpContext context, int MaterialId, int buildingid)
        {
            try
            {
                if(context is null)
                    return new ResponseList<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutLocationDTO>? model = await InventoryInfoRepository.GetLocationMaterialInventoryList(Int32.Parse(placeid), MaterialId, buildingid).ConfigureAwait(false);
                if (model is not null && model.Any())
                    return new ResponseList<InOutLocationDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<InOutLocationDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<InOutLocationDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<InOutLocationDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 출고할 품목 LIST 반환 - FRONT용
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<InOutInventoryDTO>> AddOutStoreList(HttpContext context, int roomid, int materialid, int outcount)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<InOutInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<InOutInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutInventoryDTO>? model = await InventoryInfoRepository.AddOutStoreList(Int32.Parse(placeid), roomid, materialid, outcount).ConfigureAwait(false);
                if (model is not null && model.Any())
                    return new ResponseUnit<InOutInventoryDTO>() { message = "요청이 정상 처리되었습니다.", data = model[0], code = 200 };
                else
                    return new ResponseUnit<InOutInventoryDTO>() { message = "요청이 정상 처리되었습니다.", data = new InOutInventoryDTO(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<InOutInventoryDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
