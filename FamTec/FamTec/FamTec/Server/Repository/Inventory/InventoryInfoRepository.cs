using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        public async ValueTask<bool?> AddAsync(List<InventoryTb>? model, string? GUID)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(GUID))
                    return null;

                if (model is [_, ..])
                {
                    // ADD전 상태 기록
                    List<InventoryTb>? BeforeList = await context.InventoryTbs.Where(m => m.DelYn != true && m.Occupant == GUID).ToListAsync();


                    foreach (InventoryTb Inventory in model)
                    {
                        context.InventoryTbs.Add(Inventory);
                    }

                    bool result = await context.SaveChangesAsync() > 0 ? true : false;

                    if (result)
                    {

                        List<InventoryTb>? AfterList = await context.InventoryTbs.Where(m => m.DelYn != true && m.Occupant == GUID).ToListAsync();

                        List<InventoryTb>? difference = AfterList.Except(BeforeList).ToList(); // 여기서 나온값을 SotreTB에 ADD
                        
                        foreach(InventoryTb InStoreModel in difference)
                        {
                            StoreTb Store = new StoreTb();
                            Store.Inout = 1;
                            Store.Num = InStoreModel.Num;
                            Store.Location = InStoreModel.RoomTbId;
                            Store.UnitPrice = InStoreModel.UnitPrice;
                            Store.TotalPrice = InStoreModel.Num * InStoreModel.UnitPrice;
                            Store.InoutDate = 
                        }
                        Console.WriteLine("");
                    
                    
                    }
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

                List<InventoryTb>? check = Occupant.Where(m => !String.IsNullOrWhiteSpace(m.Occupant) || !String.IsNullOrWhiteSpace(m.TimeStamp.ToString())).ToList();
                
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
                    return await context.SaveChangesAsync() > 0 ? true : false;
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

       

        public async ValueTask<bool?> SetOutInventoryInfo(List<InventoryTb?> model, int delcount,string creater, string GUID)
        {
            try
            {
                int? result = 0;

                if (model is [_, ..])
                {
                    foreach(InventoryTb? Inventory in model)
                    {
                        result += Inventory.Num;

                        if(delcount > result)
                        {
                            Inventory.Num -= Inventory.Num;
                            if(Inventory.Num == 0)
                            {
                                Inventory.TimeStamp = null;
                                Inventory.Occupant = null;
                                Inventory.DelYn = true;
                                Inventory.DelDt = DateTime.Now;
                                Inventory.DelUser = creater;
                            }
                            context.Update(Inventory);
                        }
                        else
                        {
                            if(delcount == result)
                            {
                                // 같으면 완성
                            }
                            else
                            {
                                result -= delcount;
                                Inventory.Num = result;
                                if(Inventory.Num == 0)
                                {
                                    Inventory.TimeStamp = null;
                                    Inventory.Occupant = null;
                                    Inventory.DelYn = true;
                                    Inventory.DelDt = DateTime.Now;
                                    Inventory.DelUser = creater;
                                }

                                context.Update(Inventory);
                            }
                        }
                    }
                    
                    await context.SaveChangesAsync(); // 저장

                    await RoolBackOccupant(GUID);
                    return await context.SaveChangesAsync() > 0 ? true : false; // 저장
                }
                else
                {
                    return false;
                }
            }
            catch(DBConcurrencyException ex)
            {
                // 해당 GUID 찾아서 TiemStamp / 토큰 null 해줘야함.
                await RoolBackOccupant(GUID);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                return false;
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
                        .Where(m => m.DelYn != true && m.Occupant == GUID).ToListAsync();

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
