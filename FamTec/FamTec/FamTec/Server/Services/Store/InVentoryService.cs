using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Material;
using FamTec.Server.Repository.Store;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Store
{
    public class InVentoryService : IInVentoryService
    {
        private readonly IInventoryInfoRepository InventoryInfoRepository;
        private readonly IStoreInfoRepository StoreInfoRepository;

        private ILogService LogService;

        public InVentoryService(IInventoryInfoRepository _inventoryinforepository,
            IStoreInfoRepository _storeinforepository,
            ILogService _logservice)
        {
            this.InventoryInfoRepository = _inventoryinforepository;
            this.StoreInfoRepository = _storeinforepository;
            this.LogService = _logservice;
        }
        
        /// <summary>
        /// 입고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AddInStoreService(HttpContext context, List<InOutInventoryDTO> dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                
                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string GUID = Guid.NewGuid().ToString();

                // 동시성 검사 TOKEN 넣기
                bool? SetOccupantResult = await InventoryInfoRepository.SetOccupantToken(Convert.ToInt32(placeid), dto, GUID);
                if (SetOccupantResult == false)
                {
                    // 다른곳에서 사용중인 품목
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                
                // 인벤토리 테이블에 ADD
                bool? AddInStore = await InventoryInfoRepository.AddAsync(dto, creater, Convert.ToInt32(placeid), GUID);
                if(AddInStore == true)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(AddInStore == false)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 출고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<bool?>> OutInventoryService(HttpContext context, List<InOutInventoryDTO> dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string GUID = Guid.NewGuid().ToString();

                // 동시성 검사 TOKEN 넣기
                bool? SetOccupantResult = await InventoryInfoRepository.SetOccupantToken(Convert.ToInt32(placeid), dto, GUID);
                if (SetOccupantResult == false)
                {
                    // 다른곳에서 사용중인 품목
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseList<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 404 };
                }
                if (SetOccupantResult == null)
                {
                    // 조회결과가 없을 때
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseList<bool?>() { message = "조회결과가 없습니다.", data = null, code = 200 };
                }

                bool? OutResult = await InventoryInfoRepository.SetOutInventoryInfo(dto, creater, Convert.ToInt32(placeid), GUID);
                if (OutResult == true)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseList<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if (OutResult == false)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseList<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 RESET
                    return new ResponseList<bool?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장의 입-출고 이력 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> GetPlaceInOutCountService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int count = await StoreInfoRepository.GetPlaceInOutCount(Int32.Parse(placeid));
                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 입-출고 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<InOutHistoryListDTO>> GetInOutHistoryService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutHistoryListDTO>? model = await StoreInfoRepository.GetInOutList(Convert.ToInt32(placeid));
                if (model is not null && model.Any())
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
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
        public async ValueTask<ResponseList<InOutHistoryListDTO>> GetInoutPageNationHistoryService(HttpContext context, int pagenum, int pagesize)
        {
            try
            {
                if (context is null)
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                List<InOutHistoryListDTO>? model = await StoreInfoRepository.GetInOutPageNationList(Convert.ToInt32(placeid), pagenum, pagesize);

                if (model is not null && model.Any())
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
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
        public async ValueTask<ResponseList<PeriodicInventoryRecordDTO>> PeriodicInventoryRecordService(HttpContext context, int materialid, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
           
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<PeriodicInventoryRecordDTO>? model = await InventoryInfoRepository.GetInventoryRecord(Convert.ToInt32(placeid), materialid, startDate, endDate);
                
                if (model is [_, ..])
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<PeriodicInventoryRecordDTO>() { message = "서버에서 요청을 처리하지 못하였습니다", data = null, code = 500 };
            }
        }



        /// <summary>
        /// 사업장별 재고 현황 - 공간 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaterialHistory>> GetPlaceInventoryRecordService(HttpContext context, List<int> materialid, bool type)
        {
            try
            {
                if(context is null || materialid is null)
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaterialHistory>? model = await InventoryInfoRepository.GetPlaceInventoryRecord(Convert.ToInt32(placeid), materialid, type);
                
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
                return new ResponseList<MaterialHistory>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 품목의 재고수량 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<InOutLocationDTO>> GetMaterialRoomNumService(HttpContext context, int MaterialId)
        {
            try
            {
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutLocationDTO> model = await InventoryInfoRepository.GetLocationMaterialInventoryList(Int32.Parse(placeid), MaterialId);
                return new ResponseList<InOutLocationDTO>() { message = "잘못된 요청입니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<InOutLocationDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }

   

}
