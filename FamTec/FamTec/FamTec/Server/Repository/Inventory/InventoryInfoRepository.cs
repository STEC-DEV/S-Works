using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Diagnostics;

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
        public async ValueTask<int?> AddAsync(List<InOutInventoryDTO> dto, string creater, int placeid)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            int? result = await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 디버깅 포인트를 강제로 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
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
                            await context.StoreTbs.AddAsync(Storetb);

                            bool? AddStoreResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (AddStoreResult != true)
                            {
                                await transaction.RollbackAsync();
                                return -1; // 다른곳에서 해당 품목을 사용중입니다.
                            }

                            InventoryTb Inventorytb = new InventoryTb();
                            Inventorytb.Num = InventoryDTO.AddStore.Num!.Value;
                            Inventorytb.UnitPrice = InventoryDTO.AddStore.UnitPrice!.Value;
                            Inventorytb.CreateDt = DateTime.Now;
                            Inventorytb.CreateUser = creater;
                            Inventorytb.UpdateDt = DateTime.Now;
                            Inventorytb.UpdateUser = creater;
                            Inventorytb.PlaceTbId = placeid;
                            Inventorytb.RoomTbId = InventoryDTO.AddStore.RoomID!.Value;
                            Inventorytb.MaterialTbId = InventoryDTO.MaterialID!.Value;
                            await context.InventoryTbs.AddAsync(Inventorytb);

                            int thisCurrentNum = await context.InventoryTbs
                                .Where(m => m.DelYn != true &&
                                            m.MaterialTbId == InventoryDTO.MaterialID &&
                                            m.RoomTbId == InventoryDTO.AddStore.RoomID &&
                                            m.PlaceTbId == placeid)
                                .SumAsync(m => m.Num);

                            Storetb.CurrentNum = thisCurrentNum + InventoryDTO.AddStore.Num!.Value;
                            context.Update(Storetb);
                            bool? UpdateStoreTB = await context.SaveChangesAsync() > 0 ? true : false;
                            if (UpdateStoreTB != true) // 다른곳에서 해당 품목을 사용중입니다.
                            {
                                await transaction.RollbackAsync();
                                return -1;
                            }
                        }

                        await transaction.CommitAsync();
                        return 1;
                    }
                    catch (DbUpdateConcurrencyException ex) // 다른곳에서 해당 품목을 사용중입니다.
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 기간별 입출고내역 뽑는 로직 -- 쿼리까지 완성
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<List<PeriodicDTO>?> GetInventoryRecord(int placeid,List<int> materialid, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => materialid.Contains(m.MaterialTbId) && m.DelYn != true &&
                    m.PlaceTbId == placeid &&
                    m.CreateDt >= startDate &&
                    m.CreateDt <= endDate)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();
            
                if(StoreList is null || !StoreList.Any())
                    return null;

                List<PeriodicDTO>? dtoList = (from StoreTB in StoreList
                                             join MaterialTB in context.MaterialTbs.Where(m => m.DelYn != true)
                                             on StoreTB.MaterialTbId equals MaterialTB.Id
                                             group new { StoreTB, MaterialTB } by new
                                             {
                                                 StoreTB.MaterialTbId,
                                                 MaterialTB.Code,
                                                 MaterialTB.Name
                                             } into g
                                             select new PeriodicDTO
                                             {
                                                 ID = g.Key.MaterialTbId,
                                                 Code = g.Key.Code,
                                                 Name = g.Key.Name,
                                                 InventoryList = g.Select(x => new InventoryRecord
                                                 {
                                                     INOUT_DATE = x.StoreTB.CreateDt,  // 거래일
                                                     Type = x.StoreTB.Inout,           // 입출고 구분
                                                     ID = x.StoreTB.MaterialTbId, // 품목코드
                                                     Code = x.MaterialTB.Code,
                                                     Name = x.MaterialTB.Name,    // 품목명
                                                     MaterialUnit = x.MaterialTB.Unit,    // 품목 단위
                                                     InOutNum = x.StoreTB.Num,            // 입출고 수량
                                                     InOutUnitPrice = x.StoreTB.UnitPrice, // 입출고 단가
                                                     InOutTotalPrice = x.StoreTB.TotalPrice, // 총 가격
                                                     CurrentNum = x.StoreTB.CurrentNum,   // 시점 재고 수량
                                                     Note = x.StoreTB.Note                // 비고
                                                 }).OrderBy(r => r.INOUT_DATE).ToList() // Sort by 거래일
                                             })
                             .OrderBy(dto => dto.ID)
                             .ToList();

                if (dtoList is not null && dtoList.Any())
                    return dtoList;
                else
                    return new List<PeriodicDTO>();
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
                // 전체조회
                if (type == true) 
                {
                    List<MaterialInventory>? model =
                        (from room in context.RoomTbs
                        join material in context.MaterialTbs on 1 equals 1 // Cross Join을 위해 무조껀 TRUE로 만든다.
                        where room.DelYn != true && material.DelYn != true
                              && material.PlaceTbId == placeid
                              && context.FloorTbs
                                   .Where(f => context.BuildingTbs
                                                     .Where(b => b.PlaceTbId == placeid)
                                                     .Select(b => b.Id)
                                                     .Contains(f.BuildingTbId))
                                   .Select(f => f.Id)
                                   .Contains(room.FloorTbId)
                        select new
                        {
                            R_ID = room.Id,
                            R_NM = room.Name,
                            M_ID = material.Id,
                            M_CODE = material.Code,
                            M_NM = material.Name
                        } into subQueryA // Subquery 'A'
                        join inventoryGroup in
                            (from i in context.InventoryTbs
                             where i.DelYn != true
                             group i by new { i.MaterialTbId, i.RoomTbId } into g
                             select new
                             {
                                 MATERIAL_TB_ID = g.Key.MaterialTbId,
                                 ROOM_TB_ID = g.Key.RoomTbId,
                                 TOTAL = g.Sum(x => (int?)x.Num)
                             })
                        on new { R_ID = subQueryA.R_ID, M_ID = subQueryA.M_ID } equals new { R_ID = inventoryGroup.ROOM_TB_ID, M_ID = inventoryGroup.MATERIAL_TB_ID } into joined
                        from inventory in joined.DefaultIfEmpty() // LEFT JOIN using DefaultIfEmpty
                        where MaterialIdx.Contains(subQueryA.M_ID)
                        orderby subQueryA.M_ID, subQueryA.R_ID
                        select new MaterialInventory
                        {
                            R_ID = subQueryA.R_ID,
                            R_NM = subQueryA.R_NM,
                            M_ID = subQueryA.M_ID,
                            M_CODE = subQueryA.M_CODE,
                            M_NM = subQueryA.M_NM,
                            TOTAL = inventory.TOTAL ?? 0
                        }).ToList();

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

                    List<MaterialHistory> history = new List<MaterialHistory>();
                    int resultCount = 0;
                    for (int i = 0; i < materiallist; i++)
                    {
                        MaterialHistory material = new MaterialHistory();
                        material.ID = model[resultCount].M_ID;
                        material.Code = model[resultCount].M_CODE;
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
                    List<MaterialInventory>? model =
                       (from room in context.RoomTbs
                        join material in context.MaterialTbs on 1 equals 1 // Cross Join을 위해 무조껀 TRUE로 만든다.
                        where room.DelYn != true && material.DelYn != true
                               && material.PlaceTbId == placeid
                               && context.FloorTbs
                                    .Where(f => context.BuildingTbs
                                                      .Where(b => b.PlaceTbId == placeid)
                                                      .Select(b => b.Id)
                                                      .Contains(f.BuildingTbId))
                                    .Select(f => f.Id)
                                    .Contains(room.FloorTbId)
                        select new
                        {
                            R_ID = room.Id,
                            R_NM = room.Name,
                            M_ID = material.Id,
                            M_CODE = material.Code,
                            M_NM = material.Name
                        } into subQueryA // Subquery 'A'
                        join inventoryGroup in
                             (from i in context.InventoryTbs
                            where i.DelYn != true
                            group i by new { i.MaterialTbId, i.RoomTbId } into g
                            select new
                            {
                                MATERIAL_TB_ID = g.Key.MaterialTbId,
                                ROOM_TB_ID = g.Key.RoomTbId,
                                TOTAL = g.Sum(x => (int?)x.Num)
                            })
                         on new { R_ID = subQueryA.R_ID, M_ID = subQueryA.M_ID } equals new { R_ID = inventoryGroup.ROOM_TB_ID, M_ID = inventoryGroup.MATERIAL_TB_ID } into joined
                        from inventory in joined.DefaultIfEmpty() // LEFT JOIN using DefaultIfEmpty
                        where MaterialIdx.Contains(subQueryA.M_ID)
                        orderby subQueryA.M_ID, subQueryA.R_ID
                        select new MaterialInventory
                        {
                            R_ID = subQueryA.R_ID,
                            R_NM = subQueryA.R_NM,
                            M_ID = subQueryA.M_ID,
                            M_CODE = subQueryA.M_CODE,
                            M_NM = subQueryA.M_NM,
                            TOTAL = inventory.TOTAL ?? 0
                        }).ToList();

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
                            material.Code = model[resultCount].M_CODE;

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
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delcount)
        {
            try
            {
                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs
                .Where(m => m.MaterialTbId == materialid && 
                        m.RoomTbId == roomid && 
                        m.PlaceTbId == placeid &&
                        m.DelYn != true)
                .OrderBy(m => m.CreateDt)
                .ToListAsync();

                // 개수가 뭐라도 있으면
                if (model is [_, ..])
                {
                    int? result = 0;

                    foreach (InventoryTb Inventory in model)
                    {
                        result += Inventory.Num; // 개수누적
                    }

                    if (result >= delcount) // 개수가 됨
                        return model;
                    else 
                        return null; // 개수가 부족함.
                }
                else // 개수 조회결과가 아에없음
                    return null;
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
        /// 출고 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public async ValueTask<int?> SetOutInventoryInfo(List<InOutInventoryDTO> dto, string creater, int placeid)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            int? result = await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 디버깅 포인트를 강제로 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (InOutInventoryDTO model in dto)
                        {
                            // 출고할게 여러곳에 있으니 Check 개수 Check
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value);

                            if (InventoryList is not [_, ..])
                            {
                                // 수량이 아에 없음.
                                return 0;
                            }
                            if (InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                            {
                                // 수량이 부족함.
                                return 0; 
                            }
                        }
                        // 이시점에서 변경해봄
                        foreach (InOutInventoryDTO model in dto)
                        {
                            List<InventoryTb> OutModel = new List<InventoryTb>();
                            int? result = 0;

                            // 추가해야함
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value);
                            if (InventoryList is not [_, ..])
                            {
                                // 출고개수가 부족함
                                return 0;
                            }

                            foreach (InventoryTb? inventory in InventoryList)
                            {
                                if (result <= model.AddStore.Num)
                                {
                                    OutModel.Add(inventory);
                                    result += inventory.Num;
                                    if (result == model.AddStore.Num)
                                    {
                                        break; // 반복문 종료
                                    }
                                }
                                else
                                    break; // 반복문 종료
                            }
                            // 이시점에서 변경해봄
                            if (OutModel is [_, ..])
                            {
                                if (result >= model.AddStore.Num) // 출고개수가 충분할때만 동작
                                {
                                    // 넘어온 수량이랑 실제로 빠지는 수량이랑 같은지 검사하는 CheckSum
                                    int checksum = 0;

                                    int outresult = 0; // 개수만큼 - 빼주면 됨

                                    foreach (InventoryTb OutInventoryTb in OutModel)
                                    {
                                        outresult += OutInventoryTb.Num;
                                        if (model.AddStore.Num > outresult)
                                        {
                                            checksum += OutInventoryTb.Num;

                                            OutInventoryTb.Num -= OutInventoryTb.Num;
                                            OutInventoryTb.UpdateDt = DateTime.Now;
                                            OutInventoryTb.UpdateUser = creater;

                                            if (OutInventoryTb.Num == 0)
                                            {
                                                OutInventoryTb.DelYn = true;
                                                OutInventoryTb.DelDt = DateTime.Now;
                                                OutInventoryTb.DelUser = creater;
                                            }

                                            context.Update(OutInventoryTb);
                                        }
                                        else
                                        {
                                            checksum += model.AddStore.Num!.Value - (outresult - OutInventoryTb.Num);

                                            outresult -= model.AddStore.Num!.Value;
                                            OutInventoryTb.Num = outresult;
                                            OutInventoryTb.UpdateDt = DateTime.Now;
                                            OutInventoryTb.UpdateUser = creater;

                                            if (OutInventoryTb.Num == 0)
                                            {
                                                OutInventoryTb.DelYn = true;
                                                OutInventoryTb.DelDt = DateTime.Now;
                                                OutInventoryTb.DelUser = creater;
                                            }
                                            context.Update(OutInventoryTb);
                                        }
                                    }

                                    if (checksum != model.AddStore.Num)
                                    {
                                        Console.WriteLine("결과가 다름 RollBack!");
                                        await transaction.RollbackAsync();
                                        return -1;
                                    }

                                    await context.SaveChangesAsync();
                                }

                            }

                            // Inventory 테이블에서 해당 품목의 개수 Sum
                            int thisCurrentNum = await context.InventoryTbs
                                .Where(m => m.DelYn != true &&
                                            m.MaterialTbId == model.MaterialID &&
                                            m.RoomTbId == model.AddStore.RoomID &&
                                            m.PlaceTbId == placeid)
                                            .SumAsync(m => m.Num);

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

                            await context.StoreTbs.AddAsync(store);
                        }

                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (UpdateResult)
                        {
                            await transaction.CommitAsync(); // 출고 완료
                            return 1;
                        }
                        else
                        {
                            await transaction.RollbackAsync(); // 출고실패
                            return -1;
                        }
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}"); // 다른곳에서 해당 품목을 사용중입니다.
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });

            return result;
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
                throw new ArgumentNullException();
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

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 위치 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<List<InOutLocationDTO>> GetLocationMaterialInventoryList(int placeid, int materialid, int buildingid)
        {
            try
            {

                BuildingTb? buildingtb = await context.BuildingTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.PlaceTbId == placeid && m.Id == buildingid);
                if (buildingtb is null)
                    return new List<InOutLocationDTO>();

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => m.BuildingTbId == buildingtb.Id && m.DelYn != true).ToListAsync();
                if(FloorList is null || !FloorList.Any())
                    return new List<InOutLocationDTO>();

                List<RoomTb>? RoomList = await context.RoomTbs.Where(e => FloorList.Select(m => m.Id).Contains(e.FloorTbId) && e.DelYn != true).ToListAsync();
                if (RoomList is null || !RoomList.Any())
                    return new List<InOutLocationDTO>();

                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => RoomList.Select(m => m.Id).Contains(m.RoomTbId) &&  m.DelYn != true && m.PlaceTbId == placeid && m.MaterialTbId == materialid)
                    .GroupBy(m => m.RoomTbId)
                    .Select(g => new InventoryTb
                    {
                        RoomTbId = g.Key, // 그룹의 키인 RoomId
                        PlaceTbId = g.First().PlaceTbId, // 그룹의 임의 항목에서 PlaceTbId 가져오기
                        MaterialTbId = g.First().MaterialTbId, // 그룹의 임의 항목에서 MaterialTbId 가져오기
                        Num = g.Sum(m => m.Num), // 각 그룹의 Num 필드 합계 계산
                    })
                    .ToListAsync();

                if(model is not null && model.Any())
                {
                    List<InOutLocationDTO> dto = (from InventoryTB in model
                                                  join RoomTB in context.RoomTbs.Where(m => m.DelYn != true)
                                                  on InventoryTB.RoomTbId equals RoomTB.Id
                                                  select new InOutLocationDTO
                                                  {
                                                      MaterialID = InventoryTB.MaterialTbId,
                                                      Num = InventoryTB.Num,
                                                      RoomID = InventoryTB.RoomTbId,
                                                      RoomName = RoomTB.Name
                                                  }).ToList();
                    return dto;
                }
                else
                {
                    return new List<InOutLocationDTO>();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 출고할 품목 LIST 반환 - FRONT용
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<InOutInventoryDTO>?> AddOutStoreList(int placeid,int roomid, int materialid, int outcount)
        {
            try
            {
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.RoomTbId == roomid && m.MaterialTbId == materialid).ToListAsync();

                
                if (model is null || !model.Any())
                    return null; // 개수가 부족함

                // 개수가 부족하지 않으면 로직돌아야함.
                List<InventoryTb> result = new List<InventoryTb>();
                foreach (InventoryTb InventoryTB in model)
                {
                    if(result.Select(m => m.Num).Sum() >= outcount)
                    {
                        break; // 반복문 종료
                    }
                    else
                    {
                        result.Add(InventoryTB);
                    }
                }

                int ResultCheck = result.Select(m => m.Num).Sum();
                if (ResultCheck < outcount)
                    return null; // 개수가 부족함.

                List<InventoryTb> OutResult = new List<InventoryTb>();
                int num = 0;
                foreach(InventoryTb InventoryTB in result)
                {
                    num += InventoryTB.Num;
                    if (num <= outcount)
                    {
                        OutResult.Add(InventoryTB);
                    }
                    else
                    {
                        OutResult.Add(new InventoryTb
                        {
                            Id = InventoryTB.Id,
                            RoomTbId = InventoryTB.RoomTbId,
                            MaterialTbId = InventoryTB.MaterialTbId,
                            UnitPrice = InventoryTB.UnitPrice,
                            Num = InventoryTB.Num - (num - outcount),
                            
                        });
                        break; // 반복문 종료
                    }
                }

                List<InOutInventoryDTO>? dto = (from InventoryTB in OutResult
                                                join RoomTB in context.RoomTbs.Where(m => m.DelYn != true)
                                                on InventoryTB.RoomTbId equals RoomTB.Id
                                                group new { InventoryTB, RoomTB } by new { InventoryTB.MaterialTbId, RoomTB.Id } into grouped
                                                select new InOutInventoryDTO
                                                {
                                                    InOut = 0,
                                                    MaterialID = grouped.Key.MaterialTbId,
                                                    AddStore = new AddStoreDTO
                                                    {
                                                        //InOutDate = DateTime.Now,
                                                        Note = String.Empty,
                                                        Num = grouped.Sum(x => x.InventoryTB.Num), // Sum Num for each group
                                                        RoomID = grouped.Key.Id, // RoomID from the grouped key
                                                        RoomName = grouped.First().RoomTB.Name, // RoomName from the first item in the group
                                                        UnitPrice = grouped.First().InventoryTB.UnitPrice, // UnitPrice from the first item in the group
                                                        TotalPrice = (grouped.Sum(x => x.InventoryTB.Num))*(grouped.First().InventoryTB.UnitPrice)                                                                                                 // TotalPrice can be calculated here if needed
                                                    }
                                                }).ToList();

                if (dto is not null && dto.Any())
                    return dto;
                else
                    return new List<InOutInventoryDTO>();
            }   
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    }
}
