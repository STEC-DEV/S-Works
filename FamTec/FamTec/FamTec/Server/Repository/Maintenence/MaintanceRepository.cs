using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamTec.Server.Repository.Maintenence
{
    public class MaintanceRepository : IMaintanceRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public MaintanceRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 유지보수 사용자재 등록 -- 출고등록과 비슷하다 보면됨.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddMaintanceAsync(AddMaintanceDTO dto, string creater, int placeid, string GUID)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // [1]. 토큰체크
                    foreach (InOutInventoryDTO InventoryDTO in dto.Inventory!)
                    {
                        bool isAlreadyInuser = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                                        m.RoomTbId == InventoryDTO.AddStore!.RoomID &&
                                        m.DelYn != true &&
                                        !String.IsNullOrWhiteSpace(m.RowVersion) &&
                                        m.RowVersion != GUID).AnyAsync();

                        if (isAlreadyInuser)
                            return false; // 다른곳에서 이미 사용중
                    }

                    // [2]. 수량체크
                    foreach (InOutInventoryDTO model in dto.Inventory)
                    {
                        // 출고할게 여러곳에 있으니 전체 Check 개수 Check
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value, GUID);

                        if (InventoryList is not [_, ..])
                            return null; // 수량이 아에 없음

                        if(InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                        {
                            return null; // 수량이 부족함.
                        }
                    }

                    // 유지보수 이력에 추가.
                    MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                    MaintenenceHistory.Name = dto.Name!; // 작업명
                    MaintenenceHistory.Type = dto.Type!.Value; // 작업구분 (자체작업 / 외주작업 ..)
                    MaintenenceHistory.Worker = dto.Worker!; // 작업자
                    MaintenenceHistory.UnitPrice = dto.UnitPrice!.Value; // 단가
                    MaintenenceHistory.Num = dto.Num!.Value; // 수량
                    MaintenenceHistory.TotalPrice = dto.TotalPrice!.Value; // 소요비용
                    MaintenenceHistory.CreateDt = DateTime.Now; // 생성일자
                    MaintenenceHistory.CreateUser = creater; // 생성자
                    MaintenenceHistory.UpdateDt = DateTime.Now; // 수정일자
                    MaintenenceHistory.UpdateUser = creater; // 수정자
                    MaintenenceHistory.FacilityTbId = dto.FacilityID!.Value; // 설비 ID

                    await context.MaintenenceHistoryTbs.AddAsync(MaintenenceHistory);
                    bool AddHistoryResult = await context.SaveChangesAsync() > 0 ? true : false; // 저장

                    if (!AddHistoryResult)
                    {
                        await RoolBackOccupant(GUID);
                        await transaction.RollbackAsync();
                        return null;
                    }

                    foreach (InOutInventoryDTO model in dto.Inventory)
                    {
                        List<InventoryTb> OutModel = new List<InventoryTb>();
                        int? result = 0;

                        // 출고시킬 LIST를 만든다 = 사업장ID + ROOMID + MATERIAL ID + 삭제수량 + GUID로 검색
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value, GUID);
                        if (InventoryList is not [_, ..])
                        {
                            return null; // 출고 개수가 부족함.
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

                        if (OutModel is [_, ..])
                        {
                            if (result >= model.AddStore.Num) // 출고개수가 충분할때만 동작.
                            {
                                // 개수만큼 - 빼주면 됨
                                int outresult = 0;
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
                        m.PlaceTbId == placeid).Sum(m => m.Num);

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
                        store.MaintenenceHistoryTbId = MaintenenceHistory.Id;

                        await context.StoreTbs.AddAsync(store);
                    }

                    bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(UpdateResult)
                    {
                        await transaction.CommitAsync(); // 출고완료
                        await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업.
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync(); // 출고 실패
                        await RoolBackOccupant(GUID); // 다음 작업을 위해 토큰 비우는 작업.
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
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
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async ValueTask<List<MaintanceListDTO>?> GetFacilityHistoryList(int facilityid)
        {
            try
            {
                List<MaintenenceHistoryTb>? MainTenenceList = await context.MaintenenceHistoryTbs.Where(m => m.FacilityTbId == facilityid && m.DelYn != true).ToListAsync();

                List<MaintanceListDTO> Model = new List<MaintanceListDTO>();
                if (MainTenenceList is [_, ..])
                {
                    foreach (MaintenenceHistoryTb HistoryTB in MainTenenceList)
                    {
                        MaintanceListDTO MaintanceModel = new MaintanceListDTO();
                        MaintanceModel.ID = HistoryTB.Id; // 유지보수 설비이력 인덱스
                        MaintanceModel.CreateDT = HistoryTB.CreateDt; // 생성일
                        MaintanceModel.Name = HistoryTB.Name; // 유지보수 명
                        MaintanceModel.Type = HistoryTB.Type; // 작업구분
                        MaintanceModel.TotalPrice = HistoryTB.TotalPrice; // 총 합계

                        // Log에서 반복문의 해당시점 MaintenenceHistoryTB.ID를 조회한다.
                        List<StoreTb> StoreList = await context.StoreTbs.Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id && m.DelYn != true).ToListAsync();
                        if (StoreList is [_, ..]) // 유지보수 이력이면 무조껀 있어야함.
                        {
                            foreach (StoreTb StoreTB in StoreList)
                            {
                                int MaterialID = StoreTB.MaterialTbId;
                                MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => m.Id == MaterialID && m.DelYn != true);
                                if (MaterialTB is not null)
                                {
                                    MaintanceModel.UsedMaterialList.Add(new UsedMaterialDTO
                                    {
                                        StoreID = StoreTB.Id,
                                        RoomTBID = StoreTB.RoomTbId,
                                        PlaceTBID = StoreTB.PlaceTbId,
                                        MaterialTBID = StoreTB.MaterialTbId,
                                        MaterialID = MaterialTB.Id,
                                        MaterialName = MaterialTB.Name
                                    });
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }

                        Model.Add(MaintanceModel); // 반환모델에 담는다.
                    }
                    return Model;
                }
                else
                {
                    // 해당설비는 유지보수 이력이 없음.
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 유지보수이력 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteHistoryInfo(DeleteMaintanceDTO DeleteDTO, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    MaintenenceHistoryTb? MaintenceHistoryTB = await context.MaintenenceHistoryTbs.FirstOrDefaultAsync(m => m.Id == DeleteDTO.ID && m.DelYn != true);
                    if (MaintenceHistoryTB is null)
                        return null;

                    MaintenceHistoryTB.DelDt = DateTime.Now;
                    MaintenceHistoryTB.DelYn = true;
                    MaintenceHistoryTB.DelUser = deleter;
                    MaintenceHistoryTB.Note = DeleteDTO.Note;

                    context.MaintenenceHistoryTbs.Update(MaintenceHistoryTB);
                    bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if (!UpdateResult)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    StoreTb? StoreTB = await context.StoreTbs.FirstOrDefaultAsync(m => m.Id == DeleteDTO.StoreID && m.DelYn != true);
                    if (StoreTB is null)
                        return null;

                    FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == MaintenceHistoryTB.FacilityTbId && m.DelYn != true);

                    StoreTB.DelDt = DateTime.Now;
                    StoreTB.DelYn = true;
                    StoreTB.DelUser = deleter;
                    StoreTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenceHistoryTB.Name}건 [시스템]삭제";

                    context.StoreTbs.Update(StoreTB);
                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if (!UpdateResult)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // 새로 재입고
                    InventoryTb NewInventoryTB = new InventoryTb
                    {
                        Num = StoreTB.Num,
                        UnitPrice = StoreTB.UnitPrice,
                        CreateDt = DateTime.Now,
                        CreateUser = deleter,
                        UpdateDt = DateTime.Now,
                        UpdateUser = deleter,
                        PlaceTbId = DeleteDTO.PlaceTBID!.Value,
                        RoomTbId = DeleteDTO.RoomTBID!.Value,
                        MaterialTbId = DeleteDTO.MaterialTBID!.Value
                    };

                    context.InventoryTbs.Add(NewInventoryTB);
                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if (!UpdateResult)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // 현재개수 가져와야함.
                    int thisCurrentNum = await context.InventoryTbs
                        .Where(m => m.DelYn != true &&
                                    m.MaterialTbId == DeleteDTO.MaterialTBID &&
                                    m.RoomTbId == DeleteDTO.RoomTBID &&
                                    m.PlaceTbId == DeleteDTO.PlaceTBID)
                        .SumAsync(m => m.Num);

                    // 새로 재입고
                    StoreTb NewStoreTB = new StoreTb
                    {
                        Inout = StoreTB.Inout,
                        Num = StoreTB.Num,
                        UnitPrice = StoreTB.UnitPrice,
                        TotalPrice = StoreTB.TotalPrice,
                        InoutDate = DateTime.Now,
                        CreateDt = DateTime.Now,
                        CreateUser = deleter,
                        UpdateDt = DateTime.Now,
                        UpdateUser = deleter,
                        CurrentNum = thisCurrentNum,
                        Note = StoreTB.Note,
                        PlaceTbId = DeleteDTO.PlaceTBID!.Value,
                        RoomTbId = DeleteDTO.RoomTBID!.Value,
                        MaterialTbId = DeleteDTO.MaterialTBID!.Value
                    };

                    context.StoreTbs.Add(NewStoreTB);
                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if (UpdateResult)
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
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 가지고있는 개수가 삭제할 개수보다 많은지 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="delCount"></param>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delCount, string Guid)
        {
            try
            {
                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.MaterialTbId == materialid &&
                    m.RoomTbId == roomid &&
                    m.PlaceTbId == placeid &&
                    m.RowVersion == Guid &&
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

                    if (result >= delCount) // 개수가됨
                    {
                        return model;
                    }
                    else // 개수가안됨 ROLLBACK
                    {
                        return null;
                    }
                }
                else // 개수 조회결과가 아에없음
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// SET 토큰
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> SetOccupantToken(int placeid, int roomid, int materialid, string guid)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.PlaceTbId == placeid &&
                        m.MaterialTbId == materialid &&
                        m.RoomTbId == roomid &&
                        m.DelYn != true).ToListAsync();

                    if(Occupant is [_, ..])
                    {
                        List<InventoryTb>? check = Occupant
                            .Where(m => !String.IsNullOrWhiteSpace(m.RowVersion) && m.RowVersion != guid)
                            .ToList();

                        if (check is [_, ..])
                        {
                            return false; // 다른데서 품목 사용중
                        }
                        else // 여기는 FALSE 여야 함.
                        {
                            foreach(InventoryTb item in Occupant)
                            {
                                item.RowVersion = guid;
                                context.Update(item);
                            }
                        }
                    }
                    else
                    {
                        return null;
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
                catch(DbUpdateConcurrencyException ex) // 동시성 에러
                {
                    await transaction.RollbackAsync();

                    // 해당 GUID 찾아서 TimeStamp / 토큰 null 해줘야 함.
                    await RoolBackOccupant(guid);
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; // 다른곳에서 해당 품목을 사용중입니다.
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();

                    // 해당 GUID 찾아서 TimeStamp / 토큰 null 해줘야 함.
                    await RoolBackOccupant(guid);
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }


        /// <summary>
        /// SET 토큰
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> SetOccupantToken(int placeid, AddMaintanceDTO dto, string guid)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(InOutInventoryDTO inventoryDTO in dto.Inventory!)
                    {
                        bool ItemInUse = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                            m.MaterialTbId == inventoryDTO.MaterialID &&
                            m.RoomTbId == inventoryDTO.AddStore!.RoomID &&
                            m.DelYn != true &&
                            !String.IsNullOrWhiteSpace(m.RowVersion) &&
                            m.RowVersion != guid)
                            .AnyAsync();

                        if (ItemInUse)
                            return false; // 다른곳에서 해당 품목을 사용중입니다.

                        List<InventoryTb> itemsToUpdate = await context.InventoryTbs
                            .Where(m => m.PlaceTbId == placeid &&
                                        m.MaterialTbId == inventoryDTO.MaterialID &&
                                        m.RoomTbId == inventoryDTO.AddStore!.RoomID &&
                                        m.DelYn != true).ToListAsync();

                        foreach(InventoryTb item in itemsToUpdate)
                        {
                            item.RowVersion = guid; // GUID 토큰 SET
                            context.Update(item);
                        }
                    }

                    bool result = await context.SaveChangesAsync() > 0 ? true : false;
                    if (result)
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
                catch (DbUpdateConcurrencyException ex) // 동시성 에러
                {
                    await transaction.RollbackAsync();

                    // 해당 GUID 찾아서 TimeStamp / 토큰 null 해줘야함.
                    await RoolBackOccupant(guid);
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; // 다른곳에서 해당 품목을 사용중입니다.
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    await RoolBackOccupant(guid);
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 토큰 RESET
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<Task?> RoolBackOccupant(string GUID)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(GUID))
                {
                    return null;
                }

                // 해당 코드가 없으면 RollBack Update도 ERROR
                foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
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

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid">사업장ID</param>
        /// <param name="StartDate">시작일</param>
        /// <param name="EndDate">종료일</param>
        /// <param name="Category">설비유형</param>
        /// <param name="type">작업구분</param>
        /// <returns></returns>
        public async ValueTask<List<MaintanceHistoryDTO>?> GetDateHistoryList(int placeid, DateTime StartDate, DateTime EndDate, string Category, int type)
        {
            try
            {
                List<BuildingTb>? BuildingTB = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .ToListAsync();

                if (BuildingTB is not [_, ..])
                    return null;

                List<FloorTb>? FloorTB = await context.FloorTbs.Where(m => BuildingTB.Select(m => m.Id).Contains(m.BuildingTbId) && m.DelYn != true).ToListAsync();
                if (FloorTB is not [_, ..])
                    return null;

                List<RoomTb>? RoomTB = await context.RoomTbs.Where(m => FloorTB.Select(m => m.Id).Contains(m.FloorTbId) && m.DelYn != true).ToListAsync();
                if (RoomTB is not [_, ..])
                    return null;

                // 설비유형
                List<FacilityTb>? FacilityList;
                if(Category.Equals("전체"))  // 전체
                {
                    FacilityList = await context.FacilityTbs
                       .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && m.DelYn != true)
                       .ToListAsync();
                }
                else // 그외 모두
                {
                    FacilityList = await context.FacilityTbs
                      .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && m.DelYn != true &&
                      m.Category.Equals(Category))
                      .ToListAsync();
                }
                
                List<MaintenenceHistoryTb>? HistoryList;
                // 작업구분
                if (type.Equals(0)) // 전체
                {
                    HistoryList = await context.MaintenenceHistoryTbs.Where
                      (m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                      m.CreateDt >= StartDate &&
                      m.CreateDt <= EndDate &&
                      m.DelYn != true).ToListAsync();
                }
                else // 그외 모두
                {
                    HistoryList = await context.MaintenenceHistoryTbs.Where
                        (m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                        m.CreateDt >= StartDate &&
                        m.CreateDt <= EndDate &&
                        m.DelYn != true &&
                        m.Type == type)
                        .ToListAsync();
                }

                List<MaintanceHistoryDTO> HistoryDTO = new List<MaintanceHistoryDTO>();
                // 반복문에서
                // 설비유형 일자 이력 작업구분 작업자 사용자재 소요비용
                foreach (MaintenenceHistoryTb HistoryTB in HistoryList)
                {
                    MaintanceHistoryDTO HistoryModel = new MaintanceHistoryDTO();

                    FacilityTb? FacilityModel = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == HistoryTB.FacilityTbId && m.DelYn != true);

                    HistoryModel.Category = FacilityModel!.Category; // 설비유형 --> FacilityTB 조회
                    HistoryModel.CreateDT = HistoryTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 일자 CreateDT 그냥
                    HistoryModel.HistoryTitle = HistoryTB.Name; // 이력 Name 그냥
                    HistoryModel.Type = HistoryTB.Type; // 작업구분 Type 그냥
                    HistoryModel.Worker = HistoryTB.Worker; // 작업자 Worker 그냥

                    List<StoreTb> StoreList = await context.StoreTbs
                        .Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id).ToListAsync();
                    foreach(StoreTb StoreTB in StoreList)
                    {
                        // 사용자재 --> List<StoreTB> -- MaterialID빼서 Material foreach 사용자재 넣고    
                        MaterialTb? MaterialTB = await context.MaterialTbs
                            .FirstOrDefaultAsync(m => m.Id == StoreTB.MaterialTbId);

                        HistoryModel.HistoryMaterialList.Add(new HistoryMaterialDTO
                        {
                            MaterialID = MaterialTB!.Id,
                            MaterialName = MaterialTB.Name
                        });
                    }

                    HistoryModel.TotalPrice = HistoryTB.TotalPrice; // 소요비용 - TotalPrice
                    HistoryDTO.Add(HistoryModel);
                }


                return HistoryDTO;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        public async ValueTask<List<AllMaintanceHistoryDTO>?> GetAllHistoryList(int placeid, string Category, int type)
        {
            try
            {
                List<BuildingTb>? BuildingTB = await context.BuildingTbs
                        .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                        .ToListAsync();

                if (BuildingTB is not [_, ..])
                    return null;

                List<FloorTb>? FloorTB = await context.FloorTbs.Where(m => BuildingTB.Select(m => m.Id).Contains(m.BuildingTbId) && m.DelYn != true).ToListAsync();
                if (FloorTB is not [_, ..])
                    return null;

                List<RoomTb>? RoomTB = await context.RoomTbs.Where(m => FloorTB.Select(m => m.Id).Contains(m.FloorTbId) && m.DelYn != true).ToListAsync();
                if (RoomTB is not [_, ..])
                    return null;

                // 설비유형
                List<FacilityTb>? FacilityList;
                if (Category.Equals("전체"))  // 전체
                {
                    FacilityList = await context.FacilityTbs
                       .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && m.DelYn != true)
                       .ToListAsync();
                }
                else // 그외 모두
                {
                    FacilityList = await context.FacilityTbs
                      .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && m.DelYn != true &&
                      m.Category.Equals(Category))
                      .ToListAsync();
                }

                List<MaintenenceHistoryTb>? HistoryList;
                // 작업구분
                if (type.Equals(0)) // 전체
                {
                    HistoryList = await context.MaintenenceHistoryTbs.Where
                      (m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                      m.DelYn != true).ToListAsync();
                }
                else // 그외 모두
                {
                    HistoryList = await context.MaintenenceHistoryTbs.Where
                       (m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                       m.DelYn != true &&
                       m.Type == type)
                       .ToListAsync();
                }

                var GroupHistory = HistoryList
                  .GroupBy(history => new { history.CreateDt.Year, history.CreateDt.Month })
                  .Select(group => new
                  {
                      Year = group.Key.Year,
                      Month = group.Key.Month,
                      Histories = group.ToList()
                  })
                  .ToList();


                List<AllMaintanceHistoryDTO> AllMaintanceList = new List<AllMaintanceHistoryDTO>();
                foreach (var Group in GroupHistory)
                {
                    AllMaintanceHistoryDTO GroupItem = new AllMaintanceHistoryDTO();
                    GroupItem.Years = Group.Year;
                    GroupItem.Month = Group.Month;

                    foreach (MaintenenceHistoryTb HistoryTB in Group.Histories)
                    {
                        MaintanceHistoryDTO HistoryModel = new MaintanceHistoryDTO();

                        FacilityTb? FacilityModel = await context.FacilityTbs
                            .FirstOrDefaultAsync(m => m.Id == HistoryTB.FacilityTbId && m.DelYn != true);

                        HistoryModel.Category = FacilityModel!.Category; // 설비유형 --> FacilityTB 조회
                        HistoryModel.CreateDT = HistoryTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 일자 CreateDT 그냥
                        HistoryModel.HistoryTitle = HistoryTB.Name; // 이력 Name 그냥
                        HistoryModel.Type = HistoryTB.Type; // 작업구분 Type 그냥
                        HistoryModel.Worker = HistoryTB.Worker; // 작업자 Worker 그냥

                        List<StoreTb> StoreList = await context.StoreTbs
                            .Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id).ToListAsync();
                        foreach (StoreTb StoreTB in StoreList)
                        {
                            // 사용자재 --> List<StoreTB> -- MaterialID빼서 Material foreach 사용자재 넣고    
                            MaterialTb? MaterialTB = await context.MaterialTbs
                                .FirstOrDefaultAsync(m => m.Id == StoreTB.MaterialTbId);

                            HistoryModel.HistoryMaterialList.Add(new HistoryMaterialDTO
                            {
                                MaterialID = MaterialTB!.Id,
                                MaterialName = MaterialTB.Name
                            });
                        }

                        HistoryModel.TotalPrice = HistoryTB.TotalPrice; // 소요비용 - TotalPrice
                        GroupItem.HistoryList.Add(HistoryModel);
                    }

                    AllMaintanceList.Add(GroupItem);
                }

                return AllMaintanceList;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }







    }
}
