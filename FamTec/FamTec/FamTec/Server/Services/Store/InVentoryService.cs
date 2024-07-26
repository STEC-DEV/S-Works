using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Material;
using FamTec.Server.Repository.Store;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;
using System;
using System.ComponentModel;
using System.Data;

namespace FamTec.Server.Services.Store
{
    public class InVentoryService : IInVentoryService
    {
        private readonly IInventoryInfoRepository InventoryInfoRepository;
        private readonly IMaterialInfoRepository MaterialInfoRepository;
        private readonly IStoreInfoRepository StoreInfoRepository;

        private ILogService LogService;

        public InVentoryService(IInventoryInfoRepository _inventoryinforepository,
            IMaterialInfoRepository _materialinforepository,
            IStoreInfoRepository _storeinforepository,
            ILogService _logservice)
        {
            this.InventoryInfoRepository = _inventoryinforepository;
            this.MaterialInfoRepository = _materialinforepository;
            this.StoreInfoRepository = _storeinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 입고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AddInStoreService(HttpContext? context, List<InOutInventoryDTO>? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string GUID = Guid.NewGuid().ToString();
                
                // 동시성 검사 TOKEN 넣기
                foreach(InOutInventoryDTO model in dto)
                {
                    bool? AvailableCheck = await InventoryInfoRepository.SetOccupantToken(Convert.ToInt32(placeid), model.AddStore.RoomID, model.MaterialID, GUID);
                    if(AvailableCheck != true)
                    {
                        await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                        return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 404 };
                    }
                }
                
                // 인벤토리 테이블에 ADD
                bool? AddInStore = await InventoryInfoRepository.AddAsync(dto, creater, Convert.ToInt32(placeid), GUID);
                if(AddInStore == true)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseList<InOutHistoryListDTO>> GetInOutHistoryService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<InOutHistoryListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<InOutHistoryListDTO>? model = await StoreInfoRepository.GetInOutList(Convert.ToInt32(placeid));
                if (model is [_, ..])
                {
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<InOutHistoryListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<InOutHistoryListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<int?>> GetOutCountService(HttpContext? context, int? materialid, int? roomid)
        {
            try
            {
            //if (context is null)
            //return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            //if(materialid is null)
            //return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            //if(roomid is null)
            //return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            //string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
            //if (placeid is null)
            //return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

            //List<InventoryTb>? model = await InventoryInfoRepository.GetMaterialCount(materialid, roomid, Convert.ToInt32(placeid));

            //if (model is [_, ..])
            //{
            //int result = model.Sum(i => i.Num).Value;
            //return new ResponseUnit<int?>() { message = "요청이 정상적으로 처리되었습니다.", data = result, code = 200 };
            //}
            //else
            //{
            //return new ResponseUnit<int?>() { message = "요청이 정상적으로 처리되었습니다.", data = 0, code = 200 };
            //}
                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        public async ValueTask<ResponseList<bool?>> OutInventoryService(HttpContext? context, List<InOutInventoryDTO>? dto)
        {
            try
            {
                string GUID = Guid.NewGuid().ToString();

                if (context is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (placeid is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (creater is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 동시성 검사 TOKEN 넣기
                foreach (InOutInventoryDTO model in dto)
                {
                    bool? AvailableCheck = await InventoryInfoRepository.SetOccupantToken(Convert.ToInt32(placeid), model.AddStore.RoomID, model.MaterialID, GUID);
                    if (AvailableCheck != true)

                    {
                        await InventoryInfoRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                        return new ResponseList<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 404 };
                    }
                }
                bool? OutResult = await InventoryInfoRepository.SetOutInventoryInfo(dto, creater, Convert.ToInt32(placeid), GUID);
                if(OutResult == true)
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID);
                    return new ResponseList<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if(OutResult == false)
                { 
                    await InventoryInfoRepository.RoolBackOccupant(GUID);
                    return new ResponseList<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID);
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
        /// 품목별 기간별 입출고 이력조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<PeriodicInventoryRecordDTO>> PeriodicInventoryRecordService(HttpContext? context, int? materialid, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (materialid is null)
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (startDate is null)
                    return new ResponseList<PeriodicInventoryRecordDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (endDate is null)
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
        /// 사업장별 재고 현황
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaterialHistory>?> GetPlaceInventoryRecordService(HttpContext? context, List<int>? materialid, bool? type)
        {
            try
            {
                if(context is null)
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(materialid is null)
                    return new ResponseList<MaterialHistory>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(type is null)
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

    }

   

}
