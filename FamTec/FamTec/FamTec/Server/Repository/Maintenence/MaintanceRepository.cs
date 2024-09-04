using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
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

        private IFileService FileService;
        private ILogService LogService;

        private DirectoryInfo? di;
        private string? MaintanceFileFolderPath;

        public MaintanceRepository(WorksContext _context, IFileService _fileservice, ILogService _logservice)
        {
            this.context = _context;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        public async ValueTask<DetailMaintanceDTO?> DetailMaintanceList(int MaintanceID, int placeid)
        {
            try
            {
                MaintenenceHistoryTb? maintenenceTB = await context.MaintenenceHistoryTbs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == MaintanceID && m.DelYn != true);
                if (maintenenceTB is null)
                {
                    return null;  // 유지보수 항목이 없으면 null 반환
                }

                DetailMaintanceDTO maintanceDTO = new DetailMaintanceDTO
                {
                    MaintanceID = maintenenceTB.Id, // 유지보수 ID
                    WorkDT = maintenenceTB.Workdt.ToString("yyyy-MM-dd"), // 작업일자
                    WorkName = maintenenceTB.Name, // 작업명칭
                    Type = maintenenceTB.Type, // 작업구분
                    Worker = maintenenceTB.Worker, // 작업자
                    UnitPrice = maintenenceTB.UnitPrice, // 단가
                    Num = maintenenceTB.Num, // 수량
                    TotalPrice = maintenenceTB.TotalPrice, // 합계
                    Image = !string.IsNullOrWhiteSpace(maintenenceTB.Image) ? await FileService.GetImageFile($"{Common.FileServer}\\{placeid}\\Maintance", maintenenceTB.Image) : null
                };

                var storeData = await context.StoreTbs
                    .Where(store => store.DelYn != true && store.MaintenenceHistoryTbId == maintenenceTB.Id)
                    .Select(store => new
                    {
                        Store = store,
                        Material = context.MaterialTbs.FirstOrDefault(m => m.Id == store.MaterialTbId && m.DelYn != true),
                        Room = context.RoomTbs.FirstOrDefault(r => r.Id == store.RoomTbId && r.DelYn != true)
                    })
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var data in storeData)
                {
                    if (data.Material is null || data.Room is null)
                    {
                        return null;
                    }

                    UseStoreDTO dto = new UseStoreDTO
                    {
                        MaterialID = data.Store.MaterialTbId,
                        MaterialCode = data.Material.Code,
                        MaterialName = data.Material.Name,
                        Standard = data.Material.Standard,
                        ManufacuringComp = data.Material.ManufacturingComp,
                        RoomID = data.Store.RoomTbId,
                        RoomName = data.Room.Name,
                        UnitPrice = data.Store.UnitPrice,
                        Num = data.Store.Num,
                        Unit = data.Material.Unit,
                        TotalPrice = data.Store.TotalPrice
                    };

                    maintanceDTO.UseStoreList.Add(dto);
                }

                return maintanceDTO;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 유지보수 사용자재 등록 -- 출고등록과 비슷하다 보면됨.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddMaintanceAsync(AddMaintanceDTO dto, string creater, string userid, int placeid, IFormFile? files)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    /* 수량 체크 */
                    foreach (InOutInventoryDTO model in dto.Inventory!)
                    {
                        /* 출고할게 여러곳에 있으니 전체 Check 개수 Check */
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value);

                        if (InventoryList is not [_, ..])
                            return null; /* 수량이 아에 없음 */

                        if(InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                        {
                            return null; /* 수량이 부족함. */
                        }
                    }

                    string NewFileName = files is not null ? FileService.SetNewFileName(userid.ToString(), files) : String.Empty;

                    MaintanceFileFolderPath = String.Format(@"{0}\\{1}\\Maintance", Common.FileServer, placeid.ToString());
                    di = new DirectoryInfo(MaintanceFileFolderPath);
                    if (!di.Exists) di.Create();

                    /* 유지보수 이력에 추가. -- 여기서 변경해도 동시성검사 걸림. */
                    MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                    MaintenenceHistory.Name = dto.Name!; /* 작업명 */
                    MaintenenceHistory.Type = dto.Type!.Value; /* 작업구분 (자체작업 / 외주작업 ..) */
                    MaintenenceHistory.Worker = dto.Worker!; /* 작업자 */
                    MaintenenceHistory.UnitPrice = dto.UnitPrice!.Value; /* 단가 */
                    MaintenenceHistory.Num = dto.Num!.Value; /* 수량 */
                    MaintenenceHistory.TotalPrice = dto.TotalPrice!.Value; /* 소요비용 */
                    MaintenenceHistory.CreateDt = DateTime.Now; /* 생성일자 */
                    MaintenenceHistory.CreateUser = creater; /* 생성자 */
                    MaintenenceHistory.UpdateDt = DateTime.Now; /* 수정일자 */
                    MaintenenceHistory.UpdateUser = creater; /* 수정자 */
                    MaintenenceHistory.FacilityTbId = dto.FacilityID!.Value; /* 설비 ID */
                    MaintenenceHistory.Image = files is not null ? NewFileName : null;

                    await context.MaintenenceHistoryTbs.AddAsync(MaintenenceHistory);
                    bool AddHistoryResult = await context.SaveChangesAsync() > 0 ? true : false; /* 저장 */

                    if (!AddHistoryResult)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    foreach (InOutInventoryDTO model in dto.Inventory)
                    {
                        List<InventoryTb> OutModel = new List<InventoryTb>();
                        int? result = 0;

                        /* 출고시킬 LIST를 만든다 = 사업장ID + ROOMID + MATERIAL ID + 삭제수량 */
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore!.RoomID!.Value, model.MaterialID!.Value, model.AddStore.Num!.Value);
                        if (InventoryList is not [_, ..])
                        {
                            /* 출고 개수가 부족함. */
                            return null; 
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
                                // 넘어온 수량이랑 실제로 빠지는 수량이랑 같은지 검사하는 CheckSum
                                int checksum = 0;

                                // 개수만큼 - 빼주면 됨
                                int outresult = 0;
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


                                if(checksum != model.AddStore.Num)
                                {
                                    Console.WriteLine("결과가 다름 RollBack!");
                                    await transaction.RollbackAsync();
                                }

                                await context.SaveChangesAsync();
                            }
                        }

                        // Inventory 테이블에서 해당 품목의 개수 Sum
                        int thisCurrentNum = context.InventoryTbs
                            .Where(m => m.DelYn != true &&
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
                        store.MaintenenceHistoryTbId = MaintenenceHistory.Id;

                        await context.StoreTbs.AddAsync(store);
                    }

                    bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(UpdateResult)
                    {
                        if(files is not null)
                        {
                            // 파일 넣기
                            await FileService.AddResizeImageFile(NewFileName, MaintanceFileFolderPath, files);
                        }

                        await transaction.CommitAsync(); // 출고완료
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync(); // 출고 실패
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex) // 다른곳에서 해당 품목을 사용중입니다.
                {
                    await transaction.RollbackAsync();
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return false; 
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
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
        public async ValueTask<List<MaintanceListDTO>?> GetFacilityHistoryList(int facilityid, int placeid)
        {
            try
            {
                List<MaintenenceHistoryTb>? MainTenenceList = await context.MaintenenceHistoryTbs
                    .Where(m => m.FacilityTbId == facilityid && 
                                m.DelYn != true)
                    .ToListAsync();

                List<MaintanceListDTO> Model = new List<MaintanceListDTO>();
                if (MainTenenceList is [_, ..])
                {
                    // 여기서 DTO에 이미지 변환시켜 넣어야함.
                    MaintanceFileFolderPath = String.Format(@"{0}\\{1}\\Maintance", Common.FileServer, placeid.ToString());

                    foreach (MaintenenceHistoryTb HistoryTB in MainTenenceList)
                    {
                        MaintanceListDTO MaintanceModel = new MaintanceListDTO();
                        MaintanceModel.ID = HistoryTB.Id; // 유지보수 설비이력 인덱스
                        MaintanceModel.WorkDT = HistoryTB.Workdt; // 생성일
                        MaintanceModel.Name = HistoryTB.Name; // 유지보수 명
                        MaintanceModel.Type = HistoryTB.Type; // 작업구분
                        MaintanceModel.TotalPrice = HistoryTB.TotalPrice; // 총 합계
                        MaintanceModel.Worker = HistoryTB.Worker; // 작업자
                        //MaintanceModel.Image = !string.IsNullOrWhiteSpace(HistoryTB.Image) ? await FileService.GetImageFile(MaintanceFileFolderPath, HistoryTB.Image) : null;
                        
                        // Log에서 반복문의 해당시점 MaintenenceHistoryTB.ID를 조회한다.
                        List<StoreTb> StoreList = await context.StoreTbs.Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id && m.DelYn != true).ToListAsync();
                        if (StoreList is [_, ..]) // 유지보수 이력이면 무조껀 있어야함.
                        {
                            foreach (StoreTb StoreTB in StoreList)
                            {
                                RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == StoreTB.RoomTbId);

                                MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => 
                                                                                        m.Id == StoreTB.MaterialTbId &&
                                                                                        m.DelYn != true);

                                if (MaterialTB is not null)
                                {
                                    MaintanceModel.UsedMaterialList.Add(new UsedMaterialDTO
                                    {
                                        StoreID = StoreTB.Id,
                                        RoomTBID = StoreTB.RoomTbId,
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
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delCount)
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

                    if (result >= delCount) // 개수가됨
                        return model;
                    else // 개수가안됨 ROLLBACK
                        return null;
                }
                else // 개수 조회결과가 아에없음
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
