using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Data;
using System.Diagnostics;

namespace FamTec.Server.Repository.Inventory
{
    public class InventoryInfoRepository : IInventoryInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<InventoryInfoRepository> CreateBuilderLogger;

        public InventoryInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<InventoryInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 대쉬보드용 품목별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<InventoryAmountDTO>?> GetInventoryAmountList(int placeid)
        {
            try
            {
                List<MaterialTb> MaterialList = await context.MaterialTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync().ConfigureAwait(false);
                if (MaterialList is null || !MaterialList.Any())
                    return null;

                List<int> MaterialIdx = MaterialList.Select(m => m.Id).Take(4).ToList();

                var model = await (from room in context.RoomTbs
                                                        join material in context.MaterialTbs on 1 equals 1 // Cross Join을 위해 무조건 TRUE로 만든다.
                                                        where room.DelYn != true && material.DelYn != true
                                                       && material.PlaceTbId == placeid && context.FloorTbs
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
                                                        join inventoryGroup in (from i in context.InventoryTbs
                                                                                where i.DelYn != true
                                                                                group i by new { i.MaterialTbId, i.RoomTbId } into g
                                                                                select new
                                                                                {
                                                                                    MATERIAL_TB_ID = g.Key.MaterialTbId,
                                                                                    ROOM_TB_ID = g.Key.RoomTbId,
                                                                                    TOTAL = g.Sum(x => (int?)x.Num)
                                                                                })
                                                        on new
                                                        {
                                                            R_ID = subQueryA.R_ID,
                                                            M_ID = subQueryA.M_ID
                                                        }
                                                        equals new
                                                        {
                                                            R_ID = inventoryGroup.ROOM_TB_ID,
                                                            M_ID = inventoryGroup.MATERIAL_TB_ID
                                                        } into joined
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
                                                        }).ToListAsync()
                                          .ConfigureAwait(false); // 비동기 ToListAsync() 사용

                if (model is null)
                    return null;

                // 반복문 돌리 조건 [1]
                List<RoomTb>? roomlist = await (from building in context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
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
                            .ToListAsync()
                            .ConfigureAwait(false); // 비동기 ToListAsync() 사용

                if (roomlist is null)
                    return null;

                // 반복문 돌리기 조건 [2]
                int materiallist = model.GroupBy(m => m.M_ID).Count();

                List<InventoryAmountDTO> InventoryModel = new List<InventoryAmountDTO>();
                int resultCount = 0;
                for (int i = 0; i < materiallist; i++)
                {
                    int total = 0; // 총합을 계산할 변수

                    InventoryAmountDTO material = new InventoryAmountDTO();
                    material.Name = model[resultCount].M_NM;

                    for (int j = 0; j < roomlist.Count(); j++)
                    {
                        InventoryRoomDTO dto = new InventoryRoomDTO
                        {
                            Name = model[resultCount + j].R_NM,
                            Num = model[resultCount + j].TOTAL!.Value
                        };
                        total += dto.Num; // 총합 누적
                        material.RoomInvenList!.Add(dto);
                    }
                    material.Total = total; // 자재별 총합 설정
                    resultCount += roomlist.Count();
                    InventoryModel.Add(material);
                }
                if (model.Count == resultCount)
                {
                    return InventoryModel;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }


        /// <summary>
        /// 입고로직
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public async Task<int?> AddAsync(List<InOutInventoryDTO> dto, string creater, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
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
                            //Storetb.TotalPrice = InventoryDTO.AddStore.TotalPrice!.Value;
                            Storetb.TotalPrice = InventoryDTO.AddStore!.Num!.Value * InventoryDTO.AddStore.UnitPrice!.Value;
                            Storetb.InoutDate = ThisDate;
                            Storetb.CreateDt = ThisDate;
                            Storetb.CreateUser = creater;
                            Storetb.UpdateDt = ThisDate;
                            Storetb.UpdateUser = creater;
                            Storetb.Note = InventoryDTO.AddStore.Note;
                            Storetb.RoomTbId = InventoryDTO.AddStore.RoomID!.Value;
                            Storetb.PlaceTbId = placeid;
                            Storetb.MaterialTbId = InventoryDTO.MaterialID!.Value;
                            Storetb.MaintenenceHistoryTbId = null;
                            await context.StoreTbs.AddAsync(Storetb).ConfigureAwait(false);

                            bool? AddStoreResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if (AddStoreResult != true)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return -1; // 다른곳에서 해당 품목을 사용중입니다.
                            }

                            InventoryTb Inventorytb = new InventoryTb();
                            Inventorytb.Num = InventoryDTO.AddStore.Num!.Value;
                            Inventorytb.UnitPrice = InventoryDTO.AddStore.UnitPrice!.Value;
                            Inventorytb.CreateDt = ThisDate;
                            Inventorytb.CreateUser = creater;
                            Inventorytb.UpdateDt = ThisDate;
                            Inventorytb.UpdateUser = creater;
                            Inventorytb.PlaceTbId = placeid;
                            Inventorytb.RoomTbId = InventoryDTO.AddStore.RoomID!.Value;
                            Inventorytb.MaterialTbId = InventoryDTO.MaterialID!.Value;
                            await context.InventoryTbs.AddAsync(Inventorytb).ConfigureAwait(false);

                            // 현재 개수를 가져와야함.
                            int thisCurrentNum = await context.InventoryTbs
                                .Where(m => m.DelYn != true &&
                                            m.MaterialTbId == InventoryDTO.MaterialID &&
                                            m.PlaceTbId == placeid)
                                .SumAsync(m => m.Num)
                                .ConfigureAwait(false);


                            Storetb.CurrentNum = thisCurrentNum + InventoryDTO.AddStore.Num!.Value;
                            context.Update(Storetb);
                            bool? UpdateStoreTB = await context.SaveChangesAsync() > 0 ? true : false;
                            if (UpdateStoreTB != true) // 다른곳에서 해당 품목을 사용중입니다.
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return -1;
                            }
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return 1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateConcurrencyException ex) // 다른곳에서 해당 품목을 사용중입니다.
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 기간별 입출고내역 뽑는 로직 -- 쿼리까지 완성
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<List<PeriodicDTO>?> GetInventoryRecord(int placeid,List<int> materialid, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<BuildingTb>? BuildingList = await context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync().ConfigureAwait(false);

                if (BuildingList is null || !BuildingList.Any())
                    return null;

                List<int> BuildingIdx = BuildingList.Select(m => m.Id).ToList();

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => BuildingIdx.Contains(m.BuildingTbId) && m.DelYn != true).ToListAsync();
                if (FloorList is null || !FloorList.Any())
                    return null;

                List<int> FloorIdx = FloorList.Select(m => m.Id).ToList();

                List<RoomTb>? RoomList = await context.RoomTbs.Where(m => FloorIdx.Contains(m.FloorTbId) && m.DelYn != true).ToListAsync();
                if (RoomList is null || !RoomList.Any())
                    return null;

                List<int> RoomIdx = RoomList.Select(m => m.Id).ToList();

                // 1. StoreTb 리스트를 필터링하고 가져오기
                List<StoreTb> storeList = await context.StoreTbs
                    .Where(m => materialid.Contains(m.MaterialTbId) &&
                                RoomIdx.Contains(m.RoomTbId) &&
                                m.PlaceTbId == placeid &&
                                m.CreateDt >= startDate &&
                                m.CreateDt <= endDate)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (!storeList.Any()) return null;

                // 2. 관련된 MaterialTb 정보를 가져오기
                List<int> materialIds = storeList.Select(m => m.MaterialTbId).Distinct().ToList();

                List<MaterialTb> materials = await context.MaterialTbs
                    .Where(m => materialIds.Contains(m.Id) && m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                // 3. 유지보수 이력과 시설 정보 미리 로드
                List<int> maintenenceIds = storeList
                    .Where(m => m.MaintenenceHistoryTbId != null)
                    .Select(m => m.MaintenenceHistoryTbId.Value)
                    .Distinct()
                    .ToList();

                List<MaintenenceHistoryTb> maintenanceHistories = await context.MaintenenceHistoryTbs
                    .Where(m => maintenenceIds.Contains(m.Id))
                    .ToListAsync()
                    .ConfigureAwait(false);

                List<int> facilityIds = maintenanceHistories
                    .Select(m => m.FacilityTbId)
                    .Distinct()
                    .ToList();

                List<FacilityTb> facilities = await context.FacilityTbs
                    .Where(f => facilityIds.Contains(f.Id))
                    .ToListAsync()
                    .ConfigureAwait(false);

                // 4. DTO 생성 -- StoreList 순으로 정렬해야함****
                List<PeriodicDTO> dtoList = materials.Select(material => new PeriodicDTO
                {
                    ID = material.Id,
                    Code = material.Code,
                    Name = material.Name,
                    InventoryList = storeList
                     .Where(s => s.MaterialTbId == material.Id)
                     .Select(s => new InventoryRecord
                     {
                         INOUT_DATE = s.CreateDt,
                         Type = s.Inout,
                         ID = s.Id, // storeList의 ID 사용
                         Code = material.Code,
                         Name = material.Name,
                         MaterialUnit = material.Unit,
                         InOutNum = s.Num,
                         InOutUnitPrice = s.UnitPrice,
                         InOutTotalPrice = s.TotalPrice,
                         CurrentNum = s.CurrentNum,
                         Note = s.Note,
                         MaintanceId = s.MaintenenceHistoryTbId,
                         Url = GenerateMaintenanceUrl(s.MaintenenceHistoryTbId, maintenanceHistories, facilities)
                     })
                     .OrderBy(r => r.ID)  // storeList의 ID로 내림차순 정렬
                     .ToList()
                        }).OrderBy(dto => dto.ID) // PeriodicDTO의 ID는 오름차순 정렬
             .ToList();

                return dtoList.Any() ? dtoList : new List<PeriodicDTO>();
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }


        // 유지보수 URL 생성 함수
        private string GenerateMaintenanceUrl(int? maintenenceId, List<MaintenenceHistoryTb> histories, List<FacilityTb> facilities)
        {
            try
            {
                if (maintenenceId == null) return string.Empty;

                var history = histories.FirstOrDefault(h => h.Id == maintenenceId);
                if (history == null) return string.Empty;

                var facility = facilities.FirstOrDefault(f => f.Id == history.FacilityTbId);
                if (facility == null) return string.Empty;

                return facility.Category switch
                {
                    "기계" => $"facility/machine/{facility.Id}/maintenance/{history.Id}",
                    "전기" => $"facility/electronic/{facility.Id}/maintenance/{history.Id}",
                    "승강" => $"facility/lift/{facility.Id}/maintenance/{history.Id}",
                    "소방" => $"facility/fire/{facility.Id}/maintenance/{history.Id}",
                    "건축" => $"facility/construct/{facility.Id}/maintenance/{history.Id}",
                    "통신" => $"facility/network/{facility.Id}/maintenance/{history.Id}",
                    "미화" => $"facility/beauty/{facility.Id}/maintenance/{history.Id}",
                    "보안" => $"facility/security/{facility.Id}/maintenance/{history.Id}",
                    _ => String.Empty
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 품목별 창고별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="MaterialIdx"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<MaterialHistory>?> GetPlaceInventoryRecord(int placeid, List<int> MaterialIdx, bool type)
        {
            try
            {
                // 전체조회
                if (type == true) 
                {
                    List<MaterialInventory>? model = await (from room in context.RoomTbs
                                                            join material in context.MaterialTbs on 1 equals 1 // Cross Join을 위해 무조건 TRUE로 만든다.
                                                            where room.DelYn != true && material.DelYn != true
                                                           && material.PlaceTbId == placeid && context.FloorTbs
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
                                                         join inventoryGroup in (from i in context.InventoryTbs
                                                              where i.DelYn != true
                                                              group i by new {i.MaterialTbId, i.RoomTbId } into g
                                                              select new
                                                              {
                                                                  MATERIAL_TB_ID = g.Key.MaterialTbId,
                                                                  ROOM_TB_ID = g.Key.RoomTbId,
                                                                  TOTAL = g.Sum(x => (int?)x.Num)
                                                              })
                                                         on new 
                                                         {
                                                             R_ID = subQueryA.R_ID, 
                                                             M_ID = subQueryA.M_ID 
                                                         } 
                                                         equals new 
                                                         { 
                                                             R_ID = inventoryGroup.ROOM_TB_ID,
                                                             M_ID = inventoryGroup.MATERIAL_TB_ID
                                                         } into joined
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
                                                         }).ToListAsync()
                                                         .ConfigureAwait(false); // 비동기 ToListAsync() 사용

                  
                    if (model is null)
                        return null;

                    // 반복문 돌리 조건 [1]
                    List<RoomTb>? roomlist = await (from building in context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
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
                                .ToListAsync()
                                .ConfigureAwait(false); // 비동기 ToListAsync() 사용
                  
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
                    List<MaterialInventory>? model = await (from room in context.RoomTbs
                                                            join material in context.MaterialTbs on 1 equals 1 // Cross Join을 위해 무조건 TRUE로 만든다.
                                                            where room.DelYn != true && material.DelYn != true && material.PlaceTbId == placeid && context.FloorTbs
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
                                                                on new 
                                                                { 
                                                                    R_ID = subQueryA.R_ID,
                                                                    M_ID = subQueryA.M_ID 
                                                                } 
                                                                equals new 
                                                                { 
                                                                    R_ID = inventoryGroup.ROOM_TB_ID,
                                                                    M_ID = inventoryGroup.MATERIAL_TB_ID 
                                                                } into joined
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
                                                                }).ToListAsync()
                                                                .ConfigureAwait(false);

                    model = model
                    .GroupBy(m => m.M_ID)
                    .Where(g => g.Any(m => m.TOTAL != 0))
                    .SelectMany(g => g)
                    .ToList();

                    if(model is [_, ..]) // 있으면
                    {
                        // 반복문 돌리 조건 [1]
                        List<RoomTb>? roomlist = await (from building in context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
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
                                     .ToListAsync()
                                     .ConfigureAwait(false); // 비동기 메서드 사용


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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
        public async Task<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delcount)
        {
            try
            {
                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs.Where(m => m.MaterialTbId == materialid && 
                                                                                m.RoomTbId == roomid && 
                                                                                m.PlaceTbId == placeid &&
                                                                                m.DelYn != true)
                                                                        .OrderBy(m => m.CreateDt)
                                                                        .ToListAsync()
                                                                        .ConfigureAwait(false);

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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        public async Task<InventoryTb?> GetInventoryInfo(int id)
        {
            try
            {
                InventoryTb? model = await context.InventoryTbs
                    .FirstOrDefaultAsync(m => m.Id == id)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
        public async Task<FailResult?> SetOutInventoryInfo(List<InOutInventoryDTO> dto, string creater, int placeid)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif
                /* 실패 리스트 담을곳 */
                FailResult ReturnResult = new FailResult();
                using (var transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        foreach (InOutInventoryDTO model in dto)
                        {
                            // 출고할게 여러곳에 있으니 Check 개수 Check
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value)
                            .ConfigureAwait(false);

                            if (InventoryList is null || !InventoryList.Any())
                            {
                                MaterialTb? MaterialTB = await context.MaterialTbs
                                .FirstOrDefaultAsync(m => m.Id == model.MaterialID!.Value && m.DelYn != true)
                                .ConfigureAwait(false);

                                if (MaterialTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                RoomTb? RoomTB = await context.RoomTbs
                                .FirstOrDefaultAsync(m => m.Id == model.AddStore!.RoomID!.Value && m.DelYn != true)
                                .ConfigureAwait(false);

                                if (RoomTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                int avail_Num = await context.InventoryTbs
                                .Where(m => m.PlaceTbId == placeid &&
                                m.MaterialTbId == model.MaterialID &&
                                m.RoomTbId == model.AddStore.RoomID)
                                .SumAsync(m => m.Num)
                                .ConfigureAwait(false);

                                FailInventory FailInventoryInfo = new FailInventory();
                                FailInventoryInfo.MaterialID = MaterialTB.Id;
                                FailInventoryInfo.MaterialName = MaterialTB.Name;
                                FailInventoryInfo.RoomID = RoomTB.Id;
                                FailInventoryInfo.RoomName = RoomTB.Name;
                                FailInventoryInfo.AvailableNum = avail_Num; // 가용한 수량

                                ReturnResult.FailList.Add(FailInventoryInfo);
                            }

                            if (InventoryList is [_, ..])
                            {
                                if (InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                                {
                                    // 수량이 부족함.
                                    MaterialTb? MaterialTB = await context.MaterialTbs
                                    .FirstOrDefaultAsync(m => m.Id == model.MaterialID!.Value && m.DelYn != true)
                                    .ConfigureAwait(false);

                                    if (MaterialTB is null)
                                    {
                                        ReturnResult.ReturnResult = -2;
                                        return ReturnResult;
                                    }

                                    RoomTb? RoomTB = await context.RoomTbs
                                    .FirstOrDefaultAsync(m => m.Id == model.AddStore!.RoomID!.Value && m.DelYn != true)
                                    .ConfigureAwait(false);

                                    if (RoomTB is null)
                                    {
                                        ReturnResult.ReturnResult = -2;
                                        return ReturnResult;
                                    }

                                    int avail_Num = await context.InventoryTbs
                                    .Where(m => m.PlaceTbId == placeid &&
                                    m.MaterialTbId == model.MaterialID &&
                                    m.RoomTbId == model.AddStore.RoomID)
                                    .SumAsync(m => m.Num)
                                    .ConfigureAwait(false);

                                    FailInventory FailInventoryInfo = new FailInventory();
                                    FailInventoryInfo.MaterialID = MaterialTB.Id;
                                    FailInventoryInfo.MaterialName = MaterialTB.Name;
                                    FailInventoryInfo.RoomID = RoomTB.Id;
                                    FailInventoryInfo.RoomName = RoomTB.Name;
                                    FailInventoryInfo.AvailableNum = avail_Num; // 가용한 수량

                                    ReturnResult.FailList.Add(FailInventoryInfo);
                                }
                            }
                        }

                        // 실패 List가 NULL OR COUNT = 0 이 아닌경우
                        if (ReturnResult.FailList is [_, ..])
                        {
                            ReturnResult.ReturnResult = 0;
                            return ReturnResult;
                        }

                        // 이시점에서 변경해봄
                        foreach (InOutInventoryDTO model in dto)
                        {
                            List<InventoryTb> OutModel = new List<InventoryTb>();
                            int? result = 0;

                            // 추가해야함
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value)
                            .ConfigureAwait(false);

                            if (InventoryList is not [_, ..])
                            {
                                // 출고개수가 부족함
                                ReturnResult.ReturnResult = 0;
                                return ReturnResult;
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


                            int CurrentTotalNum = 0;
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
                                            #region InventoryTB의 개수가 딱맞아서 1Row 개수 다 썻을경우

                                            int OutStoreEA = OutInventoryTb.Num;
                                            checksum += OutInventoryTb.Num;

                                            OutInventoryTb.Num -= OutInventoryTb.Num;
                                            OutInventoryTb.UpdateDt = ThisDate;
                                            OutInventoryTb.UpdateUser = creater;

                                            if (OutInventoryTb.Num == 0)
                                            {
                                                OutInventoryTb.DelYn = true;
                                                OutInventoryTb.DelDt = ThisDate;
                                                OutInventoryTb.DelUser = creater;
                                            }
                                            context.Update(OutInventoryTb);
                                            bool SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                            if (!SaveResult)
                                            {
                                                await transaction.RollbackAsync().ConfigureAwait(false);
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            CurrentTotalNum = await context.InventoryTbs
                                                               .Where(m => m.DelYn != true &&
                                                                           m.MaterialTbId == model.MaterialID &&
                                                                           m.PlaceTbId == placeid)
                                                               .SumAsync(m => m.Num)
                                                               .ConfigureAwait(false);

                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0;
                                            StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice;
                                            StoreTB.InoutDate = ThisDate;
                                            StoreTB.CreateDt = ThisDate;
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = ThisDate;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = model.AddStore.RoomID!.Value;
                                            StoreTB.MaterialTbId = model.MaterialID!.Value;
                                            StoreTB.CurrentNum = CurrentTotalNum;
                                            StoreTB.Note = model.AddStore.Note;
                                            StoreTB.PlaceTbId = placeid;

                                            await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);
                                            #endregion

                                        }
                                        else
                                        {
                                            #region InventoryTB의 개수가 딱맞지 않아서 1Row 개수중 연산을 해야 하는 경우
                                            // 사용개수가 됨.
                                            int OutStoreEA = model.AddStore.Num!.Value - (outresult - OutInventoryTb.Num);

                                            checksum += model.AddStore.Num!.Value - (outresult - OutInventoryTb.Num);

                                            outresult -= model.AddStore.Num!.Value;
                                            OutInventoryTb.Num = outresult;
                                            OutInventoryTb.UpdateDt = ThisDate;
                                            OutInventoryTb.UpdateUser = creater;

                                            if (OutInventoryTb.Num == 0)
                                            {
                                                OutInventoryTb.DelYn = true;
                                                OutInventoryTb.DelDt = ThisDate;
                                                OutInventoryTb.DelUser = creater;
                                            }
                                            context.Update(OutInventoryTb);
                                            bool SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                            if (!SaveResult)
                                            {
                                                await transaction.RollbackAsync().ConfigureAwait(false);
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            CurrentTotalNum = await context.InventoryTbs
                                           .Where(m => m.DelYn != true &&
                                                       m.MaterialTbId == model.MaterialID &&
                                                       m.PlaceTbId == placeid)
                                           .SumAsync(m => m.Num)
                                           .ConfigureAwait(false);


                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0;
                                            StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                            StoreTB.InoutDate = ThisDate;
                                            StoreTB.CreateDt = ThisDate;
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = ThisDate;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = model.AddStore.RoomID!.Value;
                                            StoreTB.MaterialTbId = model.MaterialID!.Value;
                                            StoreTB.CurrentNum = CurrentTotalNum;
                                            StoreTB.Note = model.AddStore.Note;
                                            StoreTB.PlaceTbId = placeid;
                                            await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);
                                            #endregion
                                        }
                                    }

                                    if (checksum != model.AddStore.Num)
                                    {
                                        Console.WriteLine("결과가 다름 RollBack!");
                                        await transaction.RollbackAsync().ConfigureAwait(false);
                                        ReturnResult.ReturnResult = -1;
                                        return ReturnResult;
                                    }
                                }
                            }
                        }

                        // -1 동시성에러
                        // -2 삭제된 데이터를 조회하고있음.
                        // 0 출고개수가 부족
                        // 출고완료
                        bool UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (UpdateResult)
                        {
                            ReturnResult.ReturnResult = 1;
                            await transaction.CommitAsync().ConfigureAwait(false); // 출고 완료
                            return ReturnResult;
                        }
                        else
                        {
                            // 다른곳에서 해당 품목을 사용중입니다.
                            ReturnResult.ReturnResult = -1;
                            await transaction.RollbackAsync().ConfigureAwait(false); // 출고실패
                            return ReturnResult;
                        }
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // 다른곳에서 해당 품목을 사용중입니다.
                        ReturnResult.ReturnResult = -1;
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return ReturnResult;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ReturnResult.ReturnResult = -2;
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return ReturnResult;
                    }
                }
            });
        }


        /// <summary>
        /// 사업장 - 품목ID에 해당하는 재고리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<List<InventoryTb>?> GetPlaceMaterialInventoryList(int placeid, int materialid)
        {
            try
            {
                List<InventoryTb>? InventoryList = await context.InventoryTbs
                    .Where(m => m.PlaceTbId == placeid && 
                    m.MaterialTbId == materialid && 
                    m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (InventoryList is [_, ..])
                    return InventoryList;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
           
        }

        /// <summary>
        /// 사업장 - 품목ID - 공간ID에 해당하는 품목의 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async Task<InOutLocationDTO?> GetLocationMaterialInventoryInfo(int placeid, int materialid, int roomid)
        {
            try
            {
                RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == roomid).ConfigureAwait(false);

                if (RoomTB is null)
                    return null;

                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.PlaceTbId == placeid && m.MaterialTbId == materialid && m.RoomTbId == roomid && m.DelYn != true).ToListAsync().ConfigureAwait(false);

                if(model is [_, ..])
                {
                    InOutLocationDTO dto = new InOutLocationDTO();
                    dto.MaterialID = materialid;
                    dto.Num = model.Sum(m => m.Num);
                    dto.RoomID = RoomTB.Id;
                    dto.RoomName = RoomTB.Name;
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }
        

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 위치 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<List<InOutLocationDTO>> GetLocationMaterialInventoryList(int placeid, int materialid, int buildingid)
        {
            try
            {

                BuildingTb? buildingtb = await context.BuildingTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.PlaceTbId == placeid && m.Id == buildingid).ConfigureAwait(false);
                if (buildingtb is null)
                    return new List<InOutLocationDTO>();

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => m.BuildingTbId == buildingtb.Id && m.DelYn != true).ToListAsync().ConfigureAwait(false);
                if (FloorList is null || !FloorList.Any())
                    return new List<InOutLocationDTO>();

                List<RoomTb>? RoomList = await context.RoomTbs.Where(e => FloorList.Select(m => m.Id).Contains(e.FloorTbId) && e.DelYn != true).ToListAsync().ConfigureAwait(false);
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
                    .ToListAsync()
                    .ConfigureAwait(false);

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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
        public async Task<List<InOutInventoryDTO>?> AddOutStoreList(int placeid,int roomid, int materialid, int outcount)
        {
            try
            {
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.RoomTbId == roomid && m.MaterialTbId == materialid)
                    .ToListAsync()
                    .ConfigureAwait(false);


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
                                                        Note = String.Empty,
                                                        Num = grouped.Sum(x => x.InventoryTB.Num),
                                                        RoomID = grouped.Key.Id,
                                                        RoomName = grouped.First().RoomTB.Name,
                                                        UnitPrice = grouped.First().InventoryTB.UnitPrice,
                                                        TotalPrice = (grouped.Sum(x => x.InventoryTB.Num))*(grouped.First().InventoryTB.UnitPrice)                                                                                                 // TotalPrice can be calculated here if needed
                                                    }
                                                }).ToList();

                if (dto is not null && dto.Any())
                    return dto;
                else
                    return new List<InOutInventoryDTO>();
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 이월 재고 수량
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public async Task<int> GetCarryOverNum(int placeid, int materialid, DateTime StartDate)
        {
            try
            {

                // startDate 이후의 최소 ID를 구하는 하위 쿼리
                int minIdAfterDate = await context.StoreTbs
                    .Where(x => x.PlaceTbId == placeid && x.MaterialTbId == materialid && x.CreateDt > StartDate.Date)
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                // minIdAfterDate보다 작은 ID 중에서 최대 ID를 구하는 하위 쿼리
                int maxIdBeforeMinId = await context.StoreTbs
                    .Where(x => x.PlaceTbId == placeid && x.MaterialTbId == materialid && x.Id < minIdAfterDate)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                if (maxIdBeforeMinId == 0)
                    return 0;

                // 최종 결과 가져오기
                StoreTb? result = await context.StoreTbs
                    .Where(x => x.PlaceTbId == placeid && x.MaterialTbId == materialid && x.Id == maxIdBeforeMinId)
                    .FirstOrDefaultAsync();
                
                if (result is null)
                    return 0;

                return result.CurrentNum;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 데드락 감지코드
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsDeadlockException(Exception ex)
        {
            // MySqlException 및 MariaDB의 교착 상태 오류 코드는 일반적으로 1213입니다.
            if (ex is MySqlException mysqlEx && mysqlEx.Number == 1213)
                return true;

            // InnerException에도 동일한 확인 로직을 적용
            if (ex.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1213)
                return true;

            return false;
        }

       

    }

}
