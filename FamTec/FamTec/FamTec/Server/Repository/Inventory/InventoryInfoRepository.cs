using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class TOTALTEMP
{
    public object? R_ID { get; set; }
    public object? R_NM { get; set; }
    public object? M_ID { get; set; }
    public object? M_NM { get; set; }
    public object? TOTAL { get; set; }
}

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
        /// 기간별 입출고내역 뽑는 로직 -- 쿼리까지 완성
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> GetInventoryRecord(int? placeid,int? materialid)
        {
            DateTime startDate = new DateTime(2024, 7, 24);
            DateTime endDate = new DateTime(2024, 7, 29);

            // 각날짜별 최종 INDEX 구하기 --> 해당컬럼의 INDEX가 전일재고 수량으로 들어가면됨.
            List<int> MaxID = context.Database.SqlQuery<int>
            ($"SELECT max(ID) FROM store_tb where PLACE_TB_ID = {placeid} && MATERIAL_TB_ID = {materialid} && Create_dt >= {startDate} && Create_dt <= {endDate} group by date(CREATE_DT) order by CREATE_DT desc").ToList();

            /*
                select date(create_dt) as `date`,
                  max(id) as `id`
                   from store_tb
                 WHERE place_tb_id =3 && material_tb_id = 1
                  group by date(create_dt) order by create_dt desc;
             */

            List<StoreTb>? StoreTb = context.StoreTbs
                .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.MaterialTbId == materialid)
                .ToList()
                .Where(e => e.CreateDt >= startDate && e.CreateDt <= endDate && e.DelYn != true).ToList();
            

            foreach(StoreTb store in StoreTb)
            {

            }

            Console.WriteLine("");
            return null;
        }

        

        public async ValueTask<bool?> GetInventoryRecord2(int? placeid)
        {
            // 아래 항목 LINQ로 한번 도전
            /*
             SELECT R_ID, R_NM, M_ID, M_NM, IF(B.TOTAL != '', B.TOTAL, '0') AS TOTAL
              FROM (SELECT room_tb.ID AS R_ID, room_tb.`NAME` AS R_NM, material_tb.ID AS M_ID, material_tb.`NAME` AS M_NM 
                      FROM room_tb 
                      JOIN material_tb 
                     WHERE material_tb.DEL_YN = 0 
                       AND material_tb.PLACE_TB_ID = 3
                       AND room_tb.floor_tb_id IN (SELECT id FROM floor_tb WHERE building_tb_id IN (SELECT id FROM building_tb WHERE place_tb_id = 3))
                   ) A
            LEFT JOIN (select material_tb_id, room_tb_id, sum(num) AS TOTAL from inventory_tb WHERE DEL_YN = 0 group by material_tb_id, room_tb_id) AS B
              ON A.R_ID = B.ROOM_TB_ID
             AND A.M_ID = B.material_tb_id
            ORDER BY M_ID
             */
             
            Console.WriteLine("dd");

            return null;
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
                    .Where(m => 
                    !String.IsNullOrWhiteSpace(m.Occupant) ||
                    !String.IsNullOrWhiteSpace(m.TimeStamp.ToString()))
                    .ToList()
                    .Where(m => m.Occupant != guid).ToList();

                    
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

                // 토큰 체크
                foreach (InOutInventoryDTO model in dto)
                {
                    List<InventoryTb>? Occupant = await context.InventoryTbs
                     .Where(m => m.PlaceTbId == placeid &&
                     m.RoomTbId == model.AddStore.RoomID &&
                     m.MaterialTbId == model.MaterialID &&
                     m.DelYn != true).ToListAsync();

                    List<InventoryTb>? check = Occupant
                        .Where(m => 
                        !String.IsNullOrWhiteSpace(m.Occupant) ||
                        !String.IsNullOrWhiteSpace(m.TimeStamp.ToString()))
                        .ToList().Where(m => m.Occupant != GUID).ToList();
                    
                    if (check is [_, ..])
                    {
                        // 다른곳에서 해당 토큰 사용중.
#if DEBUG
                        Console.WriteLine("다른곳에서 해당 품목을 사용중입니다.");
#endif
                        return false;
                    }
                }

                // 수량 체크
                foreach (InOutInventoryDTO model in dto)
                {
                    int? result = 0;
                    List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore.RoomID, model.MaterialID, model.AddStore.Num, GUID);
                    if(InventoryList is [_, ..])
                    {
                        foreach(InventoryTb? inventory in InventoryList)
                        {
                            if (result <= model.AddStore.Num)
                            {
                                result += inventory.Num;
                            }
                        }
                        
                        if(result < model.AddStore.Num)
                        {
#if DEBUG
                            Console.WriteLine("수량이 부족합니다");
#endif
                            await RoolBackOccupant(GUID);
                            return null;
                        }
                    }
                }

                foreach (InOutInventoryDTO model in dto)
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
                            if(result >= model.AddStore.Num) // 출고개수가 충분할때
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
                            }
                        }

                        await context.SaveChangesAsync();

                        // Inventory 테이블에서 해당 품목의 개수 Sum
                        int? thisCurrentNum = context.InventoryTbs.Where(m =>
                            m.DelYn != true &&
                            m.MaterialTbId == model.MaterialID &&
                            m.RoomTbId == model.AddStore.RoomID &&
                            m.PlaceTbId == placeid)
                                .Sum(m => m.Num);

                        if (thisCurrentNum == null)
                            thisCurrentNum = 0;

                        StoreTb store = new StoreTb();
                        store.Inout = model.InOut;
                        store.Num = model.AddStore.Num;
                        store.UnitPrice = model.AddStore.UnitPrice;
                        store.TotalPrice = model.AddStore.TotalPrice;
                        store.InoutDate = model.AddStore.InOutDate;
                        store.CreateDt = DateTime.Now;
                        store.CreateUser = creater;
                        store.UpdateDt = DateTime.Now;
                        store.UpdateUser = creater;
                        store.RoomTbId = model.AddStore.RoomID;
                        store.MaterialTbId = model.MaterialID;
                        store.CurrentNum = thisCurrentNum;
                        store.Note = model.AddStore.Note;
                        store.PlaceTbId = placeid;

                        context.StoreTbs.Add(store);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("출고개수가 부족합니다");
#endif
                        await RoolBackOccupant(GUID);
                        return null;
                    }
                }

#if DEBUG
                Console.WriteLine("출고완료");
#endif
                return true;
            }
            catch (DBConcurrencyException ex)
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
