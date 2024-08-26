using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
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

        /// <summary>
        /// 입고로직
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddAsync(List<InOutInventoryDTO> dto, string creater, int placeid, string GUID)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // [1] 토큰체크
                    foreach (InOutInventoryDTO InventoryDTO in dto)
                    {
                        List<InventoryTb>? Occupant = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                            m.RoomTbId == InventoryDTO.AddStore!.RoomID &&
                            m.DelYn != true).ToListAsync();

                        List<InventoryTb>? check = Occupant
                            .Where(m => !String.IsNullOrWhiteSpace(m.RowVersion))
                            .ToList()
                            .Where(m => m.RowVersion != GUID).ToList();

                        if (check is [_, ..]) // 다른곳에서 해당항목 사용중
                        {
                            Console.WriteLine("다른곳에서 해당 품목을 사용중입니다.");
                            return false;
                        }
                    }

                    // ADD 작업
                    foreach (InOutInventoryDTO InventoryDTO in dto)
                    {
                        StoreTb Storetb = new StoreTb();
                        Storetb.Inout = InventoryDTO.InOut!.Value;
                        Storetb.Num = InventoryDTO.AddStore!.Num!.Value;
                        Storetb.UnitPrice = InventoryDTO.AddStore.UnitPrice!.Value;
                        Storetb.TotalPrice = InventoryDTO.AddStore.TotalPrice!.Value;
                        Storetb.InoutDate = InventoryDTO.AddStore.InOutDate;
                        Storetb.CreateDt = DateTime.Now;
                        Storetb.CreateUser = creater;
                        Storetb.UpdateDt = DateTime.Now;
                        Storetb.UpdateUser = creater;
                        Storetb.Note = InventoryDTO.AddStore.Note;
                        Storetb.RoomTbId = InventoryDTO.AddStore.RoomID!.Value;
                        Storetb.PlaceTbId = placeid;
                        Storetb.MaterialTbId = InventoryDTO.MaterialID!.Value;
                        Storetb.MaintenenceHistoryTbId = null;
                        context.StoreTbs.Add(Storetb);
                        bool? AddStoreResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (AddStoreResult != true)
                        {
                            await transaction.RollbackAsync();
                            return false; // 다른곳에서 해당 품목을 사용중입니다.
                        }
                        else
                        {
                            InventoryTb Inventorytb = new InventoryTb();
                            Inventorytb.Num = InventoryDTO.AddStore.Num!.Value;
                            Inventorytb.UnitPrice = InventoryDTO.AddStore.UnitPrice!.Value;
                            Inventorytb.CreateDt = DateTime.Now;
                            Inventorytb.CreateUser = creater;
                            Inventorytb.UpdateDt = DateTime.Now;
                            Inventorytb.UpdateUser = creater;
                            Inventorytb.RowVersion = GUID;
                            Inventorytb.PlaceTbId = placeid;
                            Inventorytb.RoomTbId = InventoryDTO.AddStore.RoomID!.Value;
                            Inventorytb.MaterialTbId = InventoryDTO.MaterialID!.Value;
                            context.InventoryTbs.Add(Inventorytb);


                            int thisCurrentNum = context.InventoryTbs.Where(m => m.DelYn != true &&
                            m.MaterialTbId == InventoryDTO.MaterialID &&
                            m.RoomTbId == InventoryDTO.AddStore.RoomID &&
                            m.PlaceTbId == placeid).Sum(m => m.Num);

                            Storetb.CurrentNum = thisCurrentNum + InventoryDTO.AddStore.Num!.Value;
                            context.Update(Storetb);
                            bool? UpdateStoreTB = await context.SaveChangesAsync() > 0 ? true : false;
                            if (UpdateStoreTB != true)
                            {
                                await transaction.RollbackAsync();
                                return false; // 다른곳에서 해당 품목을 사용중입니다.
                            }
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    await RoolBackOccupant(GUID); // 토큰 RESET
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; // 다른곳에서 해당 품목을 사용중입니다.
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await RoolBackOccupant(GUID); // 토큰 RESET
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 기간별 입출고내역 뽑는 로직 -- 쿼리까지 완성
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<List<PeriodicInventoryRecordDTO>?> GetInventoryRecord(int placeid,int materialid, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                .Where(m => m.DelYn != true &&
                m.PlaceTbId == placeid &&
                m.MaterialTbId == materialid &&
                m.CreateDt >= startDate &&
                m.CreateDt <= endDate)
                .OrderBy(m => m.CreateDt)
                .ToListAsync();

                if (StoreList is [_, ..])
                {
                    List<PeriodicInventoryRecordDTO> dto = (from StoreTB in StoreList
                                                            join MaterialTB in context.MaterialTbs.Where(m => m.DelYn != true)
                                                            on StoreTB.MaterialTbId equals MaterialTB.Id
                                                            select new PeriodicInventoryRecordDTO
                                                            {
                                                                INOUT_DATE = StoreTB.CreateDt, // 거래일
                                                                Type = StoreTB.Inout, // 입출고구분
                                                                MaterialID = StoreTB.MaterialTbId, // 품목코드
                                                                MaterialName = MaterialTB.Name, // 품목명
                                                                MaterialUnit = MaterialTB.Unit, // 품목 단위
                                                                InOutNum = StoreTB.Num, // 입출고 수량
                                                                InOutUnitPrice = StoreTB.UnitPrice, // 입출고 단가
                                                                InOutTotalPrice = StoreTB.TotalPrice, // 총 가격
                                                                CurrentNum = StoreTB.CurrentNum,
                                                                Note = StoreTB.Note // 비고
                                                            }).OrderBy(m => m.INOUT_DATE)
                                                            .ToList();
                    
                    return dto;
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


        /// <summary>
        /// 품목별 창고별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="MaterialIdx"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<List<MaterialHistory>?> GetPlaceInventoryRecord(int placeid, List<int> MaterialIdx, bool type)
        {
            try
            {
                string query = string.Empty;

                // 전체조회
                if (type == true) 
                {
                    // 대상 전체조회
                    string Where = String.Empty;
                    for (int i = 0; i < MaterialIdx.Count; i++)
                    {
                        Where += $"M_ID = {MaterialIdx[i]} OR ";
                    }

                    Where = Where.Substring(0, Where.Length - 3);
                    query = $"SELECT `R_ID`, `R_NM`, `M_ID`, `M_NM`, IF(B.TOTAL != '', B.TOTAL, 0) AS TOTAL FROM (SELECT room_tb.ID AS R_ID, room_tb.`NAME` AS R_NM, material_tb.ID AS M_ID, material_tb.`NAME` AS M_NM FROM room_tb JOIN material_tb WHERE material_tb.DEL_YN = 0 AND material_tb.PLACE_TB_ID = 3 AND room_tb.floor_tb_id IN (SELECT id FROM floor_tb WHERE building_tb_id IN (SELECT id FROM building_tb WHERE place_tb_id = 3))) A LEFT JOIN (select material_tb_id, room_tb_id, sum(num) AS TOTAL from inventory_tb WHERE DEL_YN = 0 group by material_tb_id, room_tb_id) AS B ON A.R_ID = B.ROOM_TB_ID AND A.M_ID = B.material_tb_id WHERE {Where} ORDER BY M_ID, R_ID";
                    List<MaterialInventory>? model = await context.MaterialInven.FromSqlRaw<MaterialInventory>(query).ToListAsync();
                    
                    if (model is null)
                        return null;
                    
                    // 반복문 돌리 조건 [1]
                    List<RoomTb>? roomlist = (from building in context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                                              join floor in context.FloorTbs.Where(m => m.DelYn != true)
                                              on building.Id equals floor.BuildingTbId
                                              join room in context.RoomTbs.Where(m => m.DelYn != true)
                                              on floor.Id equals room.FloorTbId
                                              select new RoomTb
                                              {
                                                  Id = room.Id,
                                                  Name = room.Name,
                                                  CreateDt = room.CreateDt,
                                                  CreateUser = room.CreateUser,
                                                  UpdateDt = room.UpdateDt,
                                                  UpdateUser = room.UpdateUser,
                                                  DelYn = room.DelYn,
                                                  DelDt = room.DelDt,
                                                  DelUser = room.DelUser,
                                                  FloorTbId = room.FloorTbId
                                              }).OrderBy(m => m.CreateDt)
                                              .ToList();
                    if (roomlist is null)
                        return null;

                    // 반복문 돌리기 조건 [2]
                    int materiallist = model.GroupBy(m => m.M_ID).Count();

                    // 개수 Check
                    int checkCount = roomlist.Count() * materiallist;
                    if (checkCount != model.Count())
                        return null;


                    List<MaterialHistory> history = new List<MaterialHistory>();
                    int resultCount = 0;
                    for (int i = 0; i < materiallist; i++)
                    {
                        MaterialHistory material = new MaterialHistory();
                        material.ID = model[resultCount].M_ID;
                        material.Name = model[resultCount].M_NM;

                        //List<RoomDTO> room = new List<RoomDTO>();
                        for (int j = 0; j < roomlist.Count(); j++)
                        {
                            RoomDTO dto = new RoomDTO();
                            dto.Name = model[resultCount + j].R_NM;
                            dto.Num = model[resultCount + j].TOTAL;
                            material.RoomHistory!.Add(dto);
                        }
                        resultCount += roomlist.Count();
                        history.Add(material);
                    }
                    if (model.Count == resultCount)
                    {
                        return history;
                    }
                    else
                    {
                        return null;
                    }
                }
                else // 0이 아닌것만 조회
                {
                    string Where = String.Empty;
                    for (int i = 0; i < MaterialIdx.Count; i++)
                    {
                        Where += $"M_ID = {MaterialIdx[i]} OR ";
                    }

                    Where = Where.Substring(0, Where.Length - 3);

                    query = $"SELECT `R_ID`, `R_NM`, `M_ID`, `M_NM`, IF(B.TOTAL != '', B.TOTAL, 0) AS TOTAL FROM (SELECT room_tb.ID AS R_ID, room_tb.`NAME` AS R_NM, material_tb.ID AS M_ID, material_tb.`NAME` AS M_NM FROM room_tb JOIN material_tb WHERE material_tb.DEL_YN = 0 AND material_tb.PLACE_TB_ID = 3 AND room_tb.floor_tb_id IN (SELECT id FROM floor_tb WHERE building_tb_id IN (SELECT id FROM building_tb WHERE place_tb_id = 3))) A LEFT JOIN (select material_tb_id, room_tb_id, sum(num) AS TOTAL from inventory_tb WHERE DEL_YN = 0 group by material_tb_id, room_tb_id) AS B ON A.R_ID = B.ROOM_TB_ID AND A.M_ID = B.material_tb_id WHERE {Where} ORDER BY M_ID, R_ID";

                    List<MaterialInventory> model = await context.MaterialInven.FromSqlRaw<MaterialInventory>(query).ToListAsync();

                    model = model
                    .GroupBy(m => m.M_ID)
                    .Where(g => g.Any(m => m.TOTAL != 0))
                    .SelectMany(g => g)
                    .ToList();

                    if(model is [_, ..]) // 있으면
                    {
                        // 반복문 돌리 조건 [1]
                        List<RoomTb>? roomlist = (from building in context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                                                  join floor in context.FloorTbs.Where(m => m.DelYn != true)
                                                  on building.Id equals floor.BuildingTbId
                                                  join room in context.RoomTbs.Where(m => m.DelYn != true)
                                                  on floor.Id equals room.FloorTbId
                                                  select new RoomTb
                                                  {
                                                      Id = room.Id,
                                                      Name = room.Name,
                                                      CreateDt = room.CreateDt,
                                                      CreateUser = room.CreateUser,
                                                      UpdateDt = room.UpdateDt,
                                                      UpdateUser = room.UpdateUser,
                                                      DelYn = room.DelYn,
                                                      DelDt = room.DelDt,
                                                      DelUser = room.DelUser,
                                                      FloorTbId = room.FloorTbId
                                                  }).OrderBy(m => m.CreateDt)
                                                  .ToList();

                        int materialcount = model.GroupBy(m => m.M_ID).Count();

                        //int resultCount = 0;
                        List<MaterialHistory> history = new List<MaterialHistory>();
                        int resultCount = 0;
                        for (int i = 0; i < materialcount; i++)
                        {
                            MaterialHistory material = new MaterialHistory();
                            material.ID = model[resultCount].M_ID;
                            material.Name = model[resultCount].M_NM;

                            for (int j = 0; j < roomlist.Count(); j++)
                            {
                                RoomDTO dto = new RoomDTO();
                                dto.Name = model[resultCount + j].R_NM;
                                dto.Num = model[resultCount + j].TOTAL;
                                material.RoomHistory!.Add(dto);
                            }
                            resultCount += roomlist.Count();
                            history.Add(material);
                        }

                        if (history is [_, ..])
                            return history;
                        else
                            return new List<MaterialHistory>();
                    }
                    else // 없으면
                    {
                        return new List<MaterialHistory>();
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
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
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delcount,string GUID)
        {
            try
            {
                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs
                .Where(m => m.MaterialTbId == materialid && 
                        m.RoomTbId == roomid && 
                        m.PlaceTbId == placeid &&
                        m.RowVersion == GUID &&
                        m.DelYn != true)
                .OrderBy(m => m.CreateDt).ToListAsync();

                // 개수가 뭐라도 있으면
                if (model is [_, ..])
                {
                    int? result = 0;

                    foreach (InventoryTb Inventory in model)
                    {
                        result += Inventory.Num; // 개수누적
                    }

                    if (result >= delcount) // 개수가 됨
                    {
                        return model;
                    }
                    else // 개수가안됨. ROOLBACK
                    {
                        return null;
                    }
                }
                else // 개수 조회결과가 아에없음
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

        public async ValueTask<InventoryTb?> GetInventoryInfo(int id)
        {
            try
            {
                InventoryTb? model = await context.InventoryTbs
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// SET 토큰
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="dto"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> SetOccupantToken(int placeid, List<InOutInventoryDTO> dto, string guid)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(InOutInventoryDTO inventoryDTO in dto)
                    {
                        List<InventoryTb>? Occupant = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                            m.MaterialTbId == inventoryDTO.MaterialID &&
                            m.RoomTbId == inventoryDTO.AddStore!.RoomID &&
                            m.DelYn != true).ToListAsync();

                        if(Occupant is [_, ..])
                        {
                            List<InventoryTb>? check = Occupant
                            .Where(m =>!String.IsNullOrWhiteSpace(m.RowVersion))
                            .ToList()
                            .Where(m => m.RowVersion != guid).ToList();


                            if (check is [_, ..])
                            {
                                return false; // 다른데서 품목 사용중
                            }
                            else // 여기는 FALSE 여야 됨
                            {
                                foreach (InventoryTb OccModel in Occupant)
                                {
                                    OccModel.RowVersion = guid;
                                    context.Update(OccModel);
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    bool result = await context.SaveChangesAsync() > 0 ? true : false;
                    if(result)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // 해당 GUID 찾아서 TiemStamp / 토큰 null 해줘야함.
                    await RoolBackOccupant(guid);
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; // 다른곳에서 해당 품목을 사용중입니다.
                }
                catch (Exception ex)
                {
                    await RoolBackOccupant(guid);
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 출고 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> SetOutInventoryInfo(List<InOutInventoryDTO> dto, string creater, int placeid, string GUID)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // [1] 토큰 체크
                    foreach(InOutInventoryDTO InventoryDTO in dto)
                    {
                        List<InventoryTb>? InventoryList = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                                        m.RoomTbId == InventoryDTO.AddStore!.RoomID &&
                                        m.DelYn != true)
                                        .OrderBy(m => m.CreateDt)
                                        .ToListAsync();

                        List<InventoryTb>? RowVersionCheck = InventoryList
                                .Where(m => !String.IsNullOrWhiteSpace(m.RowVersion))
                                .ToList()
                                .Where(m => m.RowVersion != GUID).ToList();

                        if (RowVersionCheck.Count > 0)
                        {
                            return false; // 다른곳에서 해당 품목을 사용중입니다.
                        }
                    }

                    // [2]. 수량체크
                    foreach(InOutInventoryDTO model in dto)
                    {
                        // 출고할게 여러곳에 있으니 Check 개수 Check
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value, GUID);

                        if (InventoryList is not [_, ..])
                            return null; // 수량이 아에 없음.

                        if(InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                        {
                            return null; // 수량이 부족함.
                        }
                    }

                    foreach (InOutInventoryDTO model in dto)
                    {
                        List<InventoryTb> OutModel = new List<InventoryTb>();
                        int? result = 0;

                        // 추가해야함
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value, GUID);
                        if(InventoryList is not [_, ..])
                        {
                            // 출고개수가 부족함
                            return null;
                        }

                        foreach(InventoryTb? inventory in InventoryList)
                        {
                            if(result <= model.AddStore.Num)
                            {
                                OutModel.Add(inventory);
                                result += inventory.Num;
                                if(result == model.AddStore.Num)
                                {
                                    // 반복문 종료
                                    break;
                                }
                            }
                            else
                                // 반복문 종료
                                break;
                        }

                        if(OutModel is [_, ..])
                        {
                            if(result >= model.AddStore.Num) // 출고개수가 충분할때만 동작
                            {
                                int outresult = 0; // 개수만큼 - 빼주면 됨
                                
                                foreach (InventoryTb OutInventoryTb in OutModel)
                                {
                                    outresult += OutInventoryTb.Num;
                                    if (model.AddStore.Num > outresult)
                                    {
                                        OutInventoryTb.Num -= OutInventoryTb.Num;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if (OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.RowVersion = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }

                                        context.Update(OutInventoryTb);
                                    }
                                    else
                                    {
                                        outresult -= model.AddStore.Num!.Value;
                                        OutInventoryTb.Num = outresult;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if (OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.RowVersion = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }
                                        context.Update(OutInventoryTb);
                                    }
                                }
                                await context.SaveChangesAsync();
                            }
                        }

                        // Inventory 테이블에서 해당 품목의 개수 Sum
                        int thisCurrentNum = context.InventoryTbs.Where(m =>
                        m.DelYn != true &&
                        m.MaterialTbId == model.MaterialID &&
                        m.RoomTbId == model.AddStore.RoomID &&
                        m.PlaceTbId == placeid)
                            .Sum(m => m.Num);

                        StoreTb store = new StoreTb();
                        store.Inout = model.InOut!.Value;
                        store.Num = model.AddStore.Num!.Value;
                        store.UnitPrice = model.AddStore.UnitPrice!.Value;
                        store.TotalPrice = model.AddStore.TotalPrice!.Value;
                        store.InoutDate = model.AddStore.InOutDate;
                        store.CreateDt = DateTime.Now;
                        store.CreateUser = creater;
                        store.UpdateDt = DateTime.Now;
                        store.UpdateUser = creater;
                        store.RoomTbId = model.AddStore.RoomID!.Value;
                        store.MaterialTbId = model.MaterialID!.Value;
                        store.CurrentNum = thisCurrentNum;
                        store.Note = model.AddStore.Note;
                        store.PlaceTbId = placeid;

                        context.StoreTbs.Add(store);
                    }

                    bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(UpdateResult)
                    {
                        await transaction.CommitAsync(); // 출고 완료
                        await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync(); // 출고실패
                        await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업
                        return false;
                    }
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();

                    await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; // 다른곳에서 해당 품목을 사용중입니다.
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// GUID 토큰 리셋
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<Task?> RoolBackOccupant(string GUID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GUID))
                {
                    return null;
                }

                // 해당 코드가 없으면 RollBack Update도 ERROR
                foreach(var entry in context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
                {
                    //추적 안함으로 변경
                    entry.State = EntityState.Detached;
                }

                List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.DelYn != true && m.RowVersion == GUID)
                        .ToListAsync();

                if (Occupant.Any())
                {
                    foreach (InventoryTb model in Occupant)
                    {
                        model.RowVersion = null;
                        context.InventoryTbs.Update(model);
                    }
                }
                await context.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        // IN - OUT시 이용가능한지 CHECK
        public async ValueTask<bool?> AvailableCheck(int placeid, int roomid, int materialid)
        {
            try
            {
                List<InventoryTb>? Occupant = await context.InventoryTbs
                      .Where(m => m.PlaceTbId == placeid &&
                      m.RoomTbId == roomid &&
                      m.MaterialTbId == materialid &&
                      m.DelYn != true).ToListAsync();

                List<InventoryTb>? check = Occupant.Where(m => !String.IsNullOrWhiteSpace(m.RowVersion!.ToString())).ToList();

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

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 재고리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<List<InventoryTb>?> GetPlaceMaterialInventoryList(int placeid, int materialid)
        {
            try
            {
                List<InventoryTb>? InventoryList = await context.InventoryTbs
                    .Where(m => m.PlaceTbId == placeid && 
                    m.MaterialTbId == materialid && 
                    m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (InventoryList is [_, ..])
                    return InventoryList;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
           
        }
    }
}
