using DocumentFormat.OpenXml.InkML;
using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Material;
using FamTec.Server.Repository.Store;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;
using System;
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
        public async ValueTask<ResponseUnit<AddInventoryDTO>?> AddInStoreService(HttpContext? context, AddInventoryDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int? materialId = dto.MaterialID;
                if (materialId is null)
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                MaterialTb? material = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), materialId);
                if (material is null)
                    return new ResponseUnit<AddInventoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                foreach (InventoryDTO Store in dto.StoreList)
                {
                    InventoryTb Inventory = new InventoryTb();
                    Inventory.Num = Store.Num; // 수량
                    Inventory.UnitPrice = Store.UnitPrice; // 단가
                    Inventory.CreateDt = DateTime.Now;
                    Inventory.CreateUser = creater;
                    Inventory.UpdateDt = DateTime.Now;
                    Inventory.UpdateUser = creater;
                    Inventory.RoomTbId = Convert.ToInt32(Store.RoomID);
                    Inventory.PlaceTbId = Convert.ToInt32(placeid);
                    Inventory.MaterialTbId = Convert.ToInt32(materialId);

                    // 인벤토리 테이블에 ADD
                    InventoryTb? AddInventory = await InventoryInfoRepository.AddAsync(Inventory);
                    if (AddInventory is null)
                        return new ResponseUnit<AddInventoryDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

                    StoreTb newStore = new StoreTb();
                    newStore.Inout = 1; // 입고
                    newStore.Location = Store.RoomID;
                    newStore.Num = Store.Num; // 수량
                    newStore.UnitPrice = Store.UnitPrice; // 단가
                    newStore.TotalPrice = Store.Num * Store.UnitPrice; // 입출고 가격
                    newStore.InoutDate = Store.InOutDate; // 입출고날짜
                    newStore.MaterialTbId = materialId; // 품목ID
                    newStore.MaintenenceHistoryTbId = null; // 유지보수 이력ID (출고용)
                    newStore.InvenoryTbId = AddInventory.Id; // 재고ID
                    newStore.Note = Store.Note; // 비고
                    newStore.CreateDt = DateTime.Now;
                    newStore.CreateUser = creater;
                    newStore.UpdateDt = DateTime.Now;
                    newStore.UpdateUser = creater;

                    StoreTb? AddStore = await StoreInfoRepository.AddAsync(newStore);
                    if (AddStore is null)
                        return new ResponseUnit<AddInventoryDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

                }
                return new ResponseUnit<AddInventoryDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddInventoryDTO()
                {
                    MaterialID = dto.MaterialID,
                    StoreList = dto.StoreList.Select(e => new InventoryDTO
                    {
                        InOut = e.InOut,
                        InOutDate = e.InOutDate,
                        Num = e.Num,
                        RoomID = e.RoomID,
                        UnitPrice = e.UnitPrice,
                        TotalPrice = e.UnitPrice * e.Num,
                        Note = e.Note
                    }).ToList()
                }, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddInventoryDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
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

        public async ValueTask<ResponseList<bool?>> OutInventoryService(HttpContext? context, int? materialid, int? roomid)
        {
            try
            {
                int delCount = 730;
                int? result = 0;
                string GUID = Guid.NewGuid().ToString();

                List<InventoryTb> model = new List<InventoryTb>();

                if (context is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (materialid is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (roomid is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (placeid is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                string? creater = Convert.ToString(context.Items["Name"]);
                if (creater is null)
                    return new ResponseList<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                // 사업장ID + ROOMID + MATERIAL ID로 출고시킬 LIST에 GUID 토큰 박아넣음.
                bool? AvailableCheck = await InventoryInfoRepository.AvailableCheck(Int32.Parse(placeid), roomid, materialid, GUID);
                if(AvailableCheck == true)
                {
                    // 출고시킬 LIST를 만든다 = 사업장ID + ROOMID + MATERIAL ID + GUID로 검색
                    List<InventoryTb>? InventoryList = await InventoryInfoRepository.GetMaterialCount(Convert.ToInt32(placeid), roomid, materialid, delCount, GUID);
                    if (InventoryList is [_, ..])
                    {
                        foreach (InventoryTb? inventory in InventoryList)
                        {
                            if (result <= delCount)
                            {
                                model.Add(inventory);
                                result += inventory.Num;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (model is [_, ..])
                        {
                            if (result >= delCount)
                            {
                                // 개수만큼 - 빼주면 됨.
                                bool? temp = await InventoryInfoRepository.SetOutInventoryInfo(model, delCount, creater, GUID);
                                return new ResponseList<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                            }
                            else
                            {
                                await InventoryInfoRepository.RoolBackOccupant(GUID);
                                return new ResponseList<bool?>() { message = "품목의 수량이 요청 수량보다 부족합니다.", data = null, code = 200 };
                            }
                        }
                        else
                        {
                            return new ResponseList<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                        }
                    }
                    else
                    {
                        await InventoryInfoRepository.RoolBackOccupant(GUID);
                        return new ResponseList<bool?>() { message = "품목의 수량이 요청 수량보다 부족합니다.", data = null, code = 200 };
                    }
                }
                else
                {
                    await InventoryInfoRepository.RoolBackOccupant(GUID);
                    return new ResponseList<bool?>() { message = "다른 곳에서 해당 품목을 수정 중입니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        
    }

   

}
