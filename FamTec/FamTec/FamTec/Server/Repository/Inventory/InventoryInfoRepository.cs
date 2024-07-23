using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml;

namespace FamTec.Server.Repository.Inventory
{
    public class InventoryInfoRepository : IInventoryInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public InventoryInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 입고로직
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddAsync(List<InOutInventoryDTO>? model, string? creater,int? placeid, string? GUID)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(GUID))
                    return null;
                if (model is null)
                    return null;
                if (placeid is null)
                    return null;

                // 실패했을때 삭제하기 위함.
                List<InventoryTb> delInventoryTB = new List<InventoryTb>();
                List<StoreTb> delStoreTB = new List<StoreTb>();


                foreach(InOutInventoryDTO InventoryDTO in model)
                {
                    StoreTb Storetb = new StoreTb();
                    Storetb.Inout = InventoryDTO.InOut;
                    Storetb.Num = InventoryDTO.AddStore.Num;
                    Storetb.UnitPrice = InventoryDTO.AddStore.UnitPrice;
                    Storetb.TotalPrice = InventoryDTO.AddStore.TotalPrice;
                    Storetb.InoutDate = InventoryDTO.AddStore.InOutDate;
                    Storetb.CreateDt = DateTime.Now;
                    Storetb.CreateUser = creater;
                    Storetb.UpdateDt = DateTime.Now;
                    Storetb.UpdateUser = creater;
                    Storetb.Note = InventoryDTO.AddStore.Note;
                    Storetb.RoomTbId = InventoryDTO.AddStore.RoomID;
                    Storetb.PlaceTbId = placeid;
                    Storetb.MaterialTbId = InventoryDTO.MaterialID;
                    Storetb.MaintenenceHistoryTbId = null;

                    context.StoreTbs.Add(Storetb);
                    bool? AddStoreResult = await context.SaveChangesAsync() > 0 ? true : false;

                    if(AddStoreResult == true)
                    {
                        delStoreTB.Add(Storetb); // StoreTB 삭제대기열

                        InventoryTb? Inventorytb = new InventoryTb();
                        Inventorytb.Num = InventoryDTO.AddStore.Num;
                        Inventorytb.UnitPrice = InventoryDTO.AddStore.UnitPrice;
                        Inventorytb.CreateDt = DateTime.Now;
                        Inventorytb.CreateUser = creater;
                        Inventorytb.UpdateDt = DateTime.Now;
                        Inventorytb.UpdateUser = creater;
                        Inventorytb.TimeStamp = DateTime.Now;
                        Inventorytb.Occupant = GUID;
                        Inventorytb.PlaceTbId = placeid;
                        Inventorytb.RoomTbId = InventoryDTO.AddStore.RoomID;
                        Inventorytb.MaterialTbId = InventoryDTO.MaterialID;

                        context.InventoryTbs.Add(Inventorytb);
                        bool? AddInventoryResult = await context.SaveChangesAsync() > 0 ? true : false;
                        
                        if(AddInventoryResult == true)
                        {
                            delInventoryTB.Add(Inventorytb);

                            int? thisCurrentNum = context.InventoryTbs.Where(m => m.DelYn != true &&
                                            m.MaterialTbId == InventoryDTO.MaterialID &&
                                            m.RoomTbId == InventoryDTO.AddStore.RoomID &&
                                            m.PlaceTbId == placeid).Sum(m => m.Num);

                            if (thisCurrentNum is null)
                                thisCurrentNum = 0;

                            Storetb.CurrentNum = thisCurrentNum;
                            context.Update(Storetb);
                            bool? UpdateStoreTB = await context.SaveChangesAsync() > 0 ? true : false;
                            
                            if(UpdateStoreTB != true)
                            {
                                // 삭제로직
                                foreach(InventoryTb delInventory in delInventoryTB)
                                {
                                    context.Remove(delInventory);
                                }
                                foreach(StoreTb delStore in delStoreTB)
                                {
                                    context.Remove(delStore);
                                }
                                await context.SaveChangesAsync();
                                return false;
                            }
                        }
                        else
                        {
                            // 삭제로직
                            foreach (InventoryTb delInventory in delInventoryTB)
                            {
                                context.Remove(delInventory);
                            }
                            foreach (StoreTb delStore in delStoreTB)
                            {
                                context.Remove(delStore);
                            }
                            await context.SaveChangesAsync();
                            return false;
                        }
                    }
                    else
                    {
                        // 삭제로직
                        foreach (InventoryTb delInventory in delInventoryTB)
                        {
                            context.Remove(delInventory);
                        }
                        foreach (StoreTb delStore in delStoreTB)
                        {
                            context.Remove(delStore);
                        }
                        await context.SaveChangesAsync();
                        return false;
                    }
                }

                return true;
            }
            catch(DBConcurrencyException ex)
            {
                await RoolBackOccupant(GUID);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                throw new ArgumentNullException();
            }
            catch(Exception ex)
            {
                await RoolBackOccupant(GUID);
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 가지고 있는 개수가 삭제할 개수보다 많은지 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="delcount"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialid, int? delcount,string? GUID)
        {
            try
            {
                if (materialid is null)
                    return null;
                if (roomid is null)
                    return null;
                if (placeid is null)
                    return null;
                if (delcount is null)
                    return null;

                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs
                .Where(m => m.MaterialTbId == materialid && 
                        m.RoomTbId == roomid && 
                        m.PlaceTbId == placeid &&
                        m.Occupant == GUID &&
                        m.DelYn != true).OrderBy(m => m.CreateDt).ToListAsync();

                if (model is [_, ..])
                {
                    int? result = 0;

                    foreach (InventoryTb Inventory in model)
                    {
                        result += Inventory.Num;
                    }

                    if (result >= delcount)
                    {
                        return model;
                    }
                    else // 개수가안됨. ROOLBACK
                    {
                        await RoolBackOccupant(GUID);
                        return null;
                    }
                }
                    
                else
                {
                    await RoolBackOccupant(GUID);
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<InventoryTb?> GetInventoryInfo(int? id)
        {
            try
            {
                if(id is not null)
                {
                    InventoryTb? model = await context.InventoryTbs.FirstOrDefaultAsync(m => m.Id == id);
                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<bool?> SetOccupantToken(int? placeid, int? roomid, int? materialid, string? guid)
        {
            try
            {
                if (placeid is null)
                    return false;
                if (roomid is null)
                    return false;
                if (materialid is null)
                    return false;
                if (guid is null)
                    return false;

                List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.PlaceTbId == placeid &&
                        m.RoomTbId == roomid &&
                        m.MaterialTbId == materialid &&
                        m.DelYn != true).ToListAsync();

                List<InventoryTb>? check = Occupant
                    .Where(m => !String.IsNullOrWhiteSpace(m.Occupant) ||
                    !String.IsNullOrWhiteSpace(m.TimeStamp.ToString())
                    ).ToList().Where(m => m.Occupant != guid).ToList();

                    
                if(check is [_, ..])
                {
                    return false;
                }
                else
                {
                    foreach (InventoryTb OccModel in Occupant)
                    {
                        OccModel.TimeStamp = DateTime.Now;
                        OccModel.Occupant = guid;
                        context.Update(OccModel);
                    }

                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch(DBConcurrencyException ex)
            {
                // 해당 GUID 찾아서 TiemStamp / 토큰 null 해줘야함.
                await RoolBackOccupant(guid);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                await RoolBackOccupant(guid);
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

       

        /// <summary>
        /// 출고등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> SetOutInventoryInfo(List<InOutInventoryDTO?> dto, string? creater, int? placeid, string? GUID)
        {
            try
            {
                if (dto is null)
                    return null;
                if (placeid is null)
                    return null;
                if (String.IsNullOrWhiteSpace(creater))
                    return null;
                if (String.IsNullOrWhiteSpace(GUID))
                    return null;


                foreach(InOutInventoryDTO model in dto)
                {
                    List<InventoryTb> OutModel = new List<InventoryTb>();
                    int? result = 0;

                    // 출고시킬 LIST를 만든다 = 사업장ID + ROOMID + MATERIAL ID + 삭제수량 + GUID로 검색
                    List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore.RoomID, model.MaterialID, model.AddStore.Num, GUID);
                    if(InventoryList is [_, ..])
                    {
                        foreach(InventoryTb? inventory in InventoryList)
                        {
                            if(result <= model.AddStore.Num)
                            {
                                OutModel.Add(inventory);
                                result += inventory.Num;
                                if(result == model.AddStore.Num)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if(OutModel is [_, ..])
                        {
                            if(result >= model.AddStore.Num)
                            {
                                // 개수만큼 - 빼주면 됨
                                int? outresult = 0;
                                foreach (InventoryTb? OutInventoryTb in OutModel)
                                {
                                    outresult += OutInventoryTb.Num;
                                    if (model.AddStore.Num > outresult)
                                    {
                                        OutInventoryTb.Num -= OutInventoryTb.Num;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if (OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.TimeStamp = null;
                                            OutInventoryTb.Occupant = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }
                                        context.Update(OutInventoryTb);

                                    }
                                    else
                                    {
                                        outresult -= model.AddStore.Num;
                                        OutInventoryTb.Num = outresult;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if (OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.TimeStamp = null;
                                            OutInventoryTb.Occupant = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }

                                        context.Update(OutInventoryTb);

                                    }
                                }
                                // Save가 들어가면 해당 품목건에 대해서 처리가능함.

                            }
                        }
                    }
                    else
                    {
                        await RoolBackOccupant(GUID);
                        return null;
                    }
                }
                
                bool InventoryResult = await context.SaveChangesAsync() > 0 ? true : false; // 저장
                if(InventoryResult) // 성공
                {
                    // StoreTB에 현재 잔여개수 넣어야함.
                    foreach(InOutInventoryDTO model in dto)
                    {
                        int? thisCurrentNum = context.InventoryTbs.Where(m => m.DelYn != true &&
                        m.MaterialTbId == model.MaterialID &&
                        m.RoomTbId == model.AddStore.RoomID &&
                        m.PlaceTbId == placeid).Sum(m => m.Num);


                        StoreTb Storetb = new StoreTb();
                        Storetb.Inout = model.InOut; // 입출고 구분
                        Storetb.Num = model.AddStore.Num; // 수량
                        Storetb.UnitPrice = model.AddStore.UnitPrice; // 단가
                        Storetb.TotalPrice = model.AddStore.TotalPrice; // 총가격
                        Storetb.InoutDate = model.AddStore.InOutDate; // 입출고 시간
                        Storetb.CreateDt = DateTime.Now;
                        Storetb.CreateUser = creater;
                        Storetb.UpdateDt = DateTime.Now;
                        Storetb.UpdateUser = creater;
                        Storetb.Note = model.AddStore.Note; // 비고
                        Storetb.RoomTbId = model.AddStore.RoomID; // 공간ID
                        Storetb.PlaceTbId = placeid; // 사업장ID
                        Storetb.MaterialTbId = model.MaterialID; // 품목ID
                        Storetb.MaintenenceHistoryTbId = null; // 이거 재활용 되려나

                    }

                }
                else // 실패
                {
                    await RoolBackOccupant(GUID);
                    return null;
                }

            }
            catch(DBConcurrencyException ex)
            {
                // 해당 GUID 찾아서 TiemStamp / 토큰 null 해줘야함.
                await RoolBackOccupant(GUID);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                throw new ArgumentNullException();
            }
            catch (Exception ex)
            {
                await RoolBackOccupant(GUID);
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        public async ValueTask<Task?> RoolBackOccupant(string GUID)
        {
            try
            {
                if (GUID is null)
                    return null;

                List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.DelYn != true && m.Occupant == GUID)
                        .ToListAsync();

                if (Occupant is [_, ..])
                {
                    foreach (InventoryTb model in Occupant)
                    {
                        model.Occupant = null;
                        model.TimeStamp = null;

                        context.InventoryTbs.Update(model);
                    }
                }
                await context.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }

        // IN - OUT시 이용가능한지 CHECK
        public async ValueTask<bool?> AvailableCheck(int? placeid, int? roomid, int? materialid)
        {
            try
            {
                if (placeid is null)
                    return null;
                
                if (roomid is null)
                    return null;
                
                if (materialid is null)
                    return null;

                List<InventoryTb>? Occupant = await context.InventoryTbs
                      .Where(m => m.PlaceTbId == placeid &&
                      m.RoomTbId == roomid &&
                      m.MaterialTbId == materialid &&
                      m.DelYn != true).ToListAsync();

                List<InventoryTb>? check = Occupant.Where(m => !String.IsNullOrWhiteSpace(m.Occupant) || !String.IsNullOrWhiteSpace(m.TimeStamp.ToString())).ToList();

                if (check is [_, ..])
                    return false;
                else
                    return true;

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }
    }
}
