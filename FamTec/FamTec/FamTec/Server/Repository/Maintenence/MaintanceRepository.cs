using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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


        /// <summary>
        /// 유지보수 ID로 유지보수 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<MaintenenceHistoryTb> GetMaintenanceInfo(int id)
        {
            try
            {
                MaintenenceHistoryTb? model = await context.MaintenenceHistoryTbs.FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true);

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
        /// 유지보수 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdateMaintenanceInfo(MaintenenceHistoryTb model)
        {
            try
            {
                context.MaintenenceHistoryTbs.Update(model);
                
                bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                
                if (UpdateResult)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
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
                    TotalPrice = maintenenceTB.TotalPrice, // 합계
                    Image = !string.IsNullOrWhiteSpace(maintenenceTB.Image) ? await FileService.GetImageFile($"{Common.FileServer}\\{placeid}\\Maintance", maintenenceTB.Image) : null
                };

                var result = await (from UseMaintenenceTB in context.UseMaintenenceMaterialTbs
                                    where UseMaintenenceTB.DelYn != true && UseMaintenenceTB.MaintenanceTbId == maintenenceTB.Id && UseMaintenenceTB.DelYn != true && UseMaintenenceTB.Inout == 0
                                    join RoomTB in context.RoomTbs on UseMaintenenceTB.RoomTbId equals RoomTB.Id
                                    join MaterialTB in context.MaterialTbs on UseMaintenenceTB.MaterialTbId equals MaterialTB.Id
                                    where RoomTB.DelYn != true && MaterialTB.DelYn != true
                                    select new
                                    {
                                        UseMaintenenceMaterial = UseMaintenenceTB,
                                        Room = RoomTB,
                                        Material = MaterialTB
                                    }).ToListAsync();

                foreach(var item in result)
                {
                    if(item.Material is null || item.Room is null)
                    {
                        return null;
                    }

                    UseMaterialDTO dto = new UseMaterialDTO
                    {
                        ID = item.UseMaintenenceMaterial.Id,
                        MaterialID = item.Material.Id,
                        MaterialCode = item.Material.Code,
                        MaterialName = item.Material.Name,
                        Standard = item.Material.Standard,
                        ManufacuringComp = item.Material.ManufacturingComp,
                        RoomID = item.Room.Id,
                        RoomName = item.Room.Name,
                        //StoreID = item.Store.Id,
                        UnitPrice = item.UseMaintenenceMaterial.Unitprice,
                        Num = item.UseMaintenenceMaterial.Num,
                        TotalPrice = item.UseMaintenenceMaterial.Totalprice,
                        Unit = item.Material.Unit
                    };

                    maintanceDTO.UseMaterialList.Add(dto);
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
        /// 이미지 업로드
        /// </summary>
        /// <param name="id"></param>
        /// <param name="placeid"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddMaintanceImageAsync(int id, int placeid, IFormFile? files)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    string? NewFileName = files is not null ? FileService.SetNewFileName(id.ToString(), files) : String.Empty;

                    MaintenenceHistoryTb? MaintenenceHistory = await context.MaintenenceHistoryTbs
                        .FirstOrDefaultAsync(m => m.Id == id &&
                                                  m.DelYn != true);
                    
                    if (MaintenenceHistory is null)
                        return null;

                    MaintanceFileFolderPath = String.Format(@"{0}\\{1}\\Maintance", Common.FileServer, placeid.ToString());
                    di = new DirectoryInfo(MaintanceFileFolderPath);
                    if (!di.Exists) di.Create();

                    MaintenenceHistory.Image = files is not null ? NewFileName : null;

                    context.MaintenenceHistoryTbs.Update(MaintenenceHistory);
                    bool UpdateImageResult = await context.SaveChangesAsync() > 0 ? true : false;

                    if (UpdateImageResult)
                    {
                        if (files is not null)
                        {
                            // 파일 넣기
                            await FileService.AddResizeImageFile(NewFileName, MaintanceFileFolderPath, files);
                        }

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
                    await transaction.RollbackAsync(); // 트랜잭션 롤백
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 유지보수 사용자재 등록 -- 출고등록과 비슷하다 보면됨.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<int?> AddMaintanceAsync(AddMaintenanceDTO dto, string creater, string userid, int placeid)
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
                            return -1; /* 수량이 아에 없음 */

                        if(InventoryList.Sum(i => i.Num) < model.AddStore.Num)
                        {
                            return -1; /* 수량이 부족함. */
                        }
                    }

                    /* 유지보수 이력에 추가. -- 여기서 변경해도 동시성검사 걸림. */
                    MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                    MaintenenceHistory.Name = dto.Name!; /* 작업명 */
                    MaintenenceHistory.Type = dto.Type!.Value; /* 작업구분 (자체작업 / 외주작업 ..) */
                    MaintenenceHistory.Worker = dto.Worker!; /* 작업자 */
                    MaintenenceHistory.Workdt = dto.WorkDT; // 작업일자
                    MaintenenceHistory.TotalPrice = dto.TotalPrice!.Value; /* 소요비용 */
                    MaintenenceHistory.CreateDt = DateTime.Now; /* 생성일자 */
                    MaintenenceHistory.CreateUser = creater; /* 생성자 */
                    MaintenenceHistory.UpdateDt = DateTime.Now; /* 수정일자 */
                    MaintenenceHistory.UpdateUser = creater; /* 수정자 */
                    MaintenenceHistory.FacilityTbId = dto.FacilityID!.Value; /* 설비 ID */
                    
                    await context.MaintenenceHistoryTbs.AddAsync(MaintenenceHistory);
                    bool AddHistoryResult = await context.SaveChangesAsync() > 0 ? true : false; /* 저장 */

                    if (!AddHistoryResult)
                    {
                        await transaction.RollbackAsync();
                        return 0;
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
                            return -1; 
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
                                    return 0;
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

                        UseMaintenenceMaterialTb MaintenenceMaterialTB = new UseMaintenenceMaterialTb();
                        MaintenenceMaterialTB.Inout = model.InOut!.Value;
                        MaintenenceMaterialTB.Unitprice = model.AddStore.UnitPrice!.Value;
                        MaintenenceMaterialTB.Num = model.AddStore.Num!.Value;
                        MaintenenceMaterialTB.Totalprice = model.AddStore.TotalPrice!.Value;
                        MaintenenceMaterialTB.MaterialTbId = model.MaterialID!.Value;
                        MaintenenceMaterialTB.RoomTbId = model.AddStore.RoomID!.Value;
                        MaintenenceMaterialTB.MaintenanceTbId = MaintenenceHistory.Id;
                        MaintenenceMaterialTB.CreateDt = DateTime.Now;
                        MaintenenceMaterialTB.CreateUser = creater;
                        MaintenenceMaterialTB.UpdateDt = DateTime.Now;
                        MaintenenceMaterialTB.PlaceTbId = placeid;
                        MaintenenceMaterialTB.Note = model.AddStore.Note;

                        await context.UseMaintenenceMaterialTbs.AddAsync(MaintenenceMaterialTB);
                        bool AddMaintenenceMaterialResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!AddMaintenenceMaterialResult)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

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
                        store.MaintenenceMaterialTbId = MaintenenceMaterialTB.Id;

                        await context.StoreTbs.AddAsync(store);
                        bool AddStoreResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(!AddStoreResult)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

                     
                    }

                    await transaction.CommitAsync(); // 출고완료
                    return MaintenenceHistory.Id; // ID 반환
                }
                catch (DbUpdateConcurrencyException ex) // 다른곳에서 해당 품목을 사용중입니다.
                {
                    await transaction.RollbackAsync();
                    LogService.LogMessage($"동시성 에러 {ex.Message}");
                    return 0; 
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
                        
                        // Log에서 반복문의 해당시점 MaintenenceHistoryTB.ID를 조회한다.
                        List<StoreTb> StoreList = await context.StoreTbs.Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id && 
                                                                                    m.DelYn != true && 
                                                                                    m.Inout == 0)
                                                    .ToListAsync();

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
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> deleteMaintenanceStoreRecord(List<DeleteMaintanceDTO> DeleteDTO, int placeid, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (DeleteMaintanceDTO dto in DeleteDTO)
                    {
                        MaintenenceHistoryTb? MaintenceHistoryTB = await context.MaintenenceHistoryTbs.FirstOrDefaultAsync(m => m.Id == dto.MaintanceID && m.DelYn != true);
                        if (MaintenceHistoryTB is null)
                            return null;

                        FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == MaintenceHistoryTB.FacilityTbId && m.DelYn != true);
                        if(FacilityTB is null)
                            return null;

                        StoreTb? StoreTB = await context.StoreTbs.FirstOrDefaultAsync(m => m.Id == dto.StoreID && m.DelYn != true && m.Inout == 0);
                        if (StoreTB is null)
                            return null;

                        StoreTB.DelDt = DateTime.Now;
                        StoreTB.DelYn = true;
                        StoreTB.DelUser = deleter;
                        StoreTB.Note = dto.Note;
                        StoreTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenceHistoryTB.Name}건 [시스템]삭제";
                        
                        context.StoreTbs.Update(StoreTB);
                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }

                        UseMaintenenceMaterialTb? UseMaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs.FirstOrDefaultAsync(m => m.Id == dto.UseMaintenenceID && m.DelYn != true);
                        if (UseMaintenenceMaterialTB is null)
                            return null;

                        UseMaintenenceMaterialTB.DelDt = DateTime.Now;
                        UseMaintenenceMaterialTB.DelYn = true;
                        UseMaintenenceMaterialTB.DelUser = deleter;
                        UseMaintenenceMaterialTB.Note = dto.Note;
                        UseMaintenenceMaterialTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenceHistoryTB.Name}건 [시스템]삭제";

                        context.UseMaintenenceMaterialTbs.Update(UseMaintenenceMaterialTB);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(!UpdateResult)
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
                            PlaceTbId = placeid,
                            RoomTbId = dto.RoomTBID!.Value,
                            MaterialTbId = dto.MaterialTBID!.Value
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
                                        m.MaterialTbId == dto.MaterialTBID &&
                                        m.RoomTbId == dto.RoomTBID &&
                                        m.PlaceTbId == placeid)
                            .SumAsync(m => m.Num);

                        // 새로 재입고
                        UseMaintenenceMaterialTb NewUseMaintenenceMaterialTB = new UseMaintenenceMaterialTb
                        {
                            Inout = 1,
                            Num = UseMaintenenceMaterialTB.Num,
                            Unitprice = UseMaintenenceMaterialTB.Unitprice,
                            Totalprice = UseMaintenenceMaterialTB.Totalprice,
                            CreateDt = DateTime.Now,
                            CreateUser = deleter,
                            UpdateDt = DateTime.Now,
                            UpdateUser = deleter,
                            PlaceTbId = placeid,
                            RoomTbId = dto.RoomTBID!.Value,
                            MaterialTbId = dto.MaterialTBID!.Value,
                            Note2 = $"{FacilityTB!.Name}설비의 {MaintenceHistoryTB.Name}건 [시스템]재입고",
                            MaintenanceTbId = MaintenceHistoryTB.Id
                        };

                        context.UseMaintenenceMaterialTbs.Add(NewUseMaintenenceMaterialTB);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }

                        // 새로 재입고
                        StoreTb NewStoreTB = new StoreTb
                        {
                            Inout = 1,
                            Num = StoreTB.Num,
                            UnitPrice = StoreTB.UnitPrice,
                            TotalPrice = StoreTB.TotalPrice,
                            InoutDate = DateTime.Now,
                            CreateDt = DateTime.Now,
                            CreateUser = deleter,
                            UpdateDt = DateTime.Now,
                            UpdateUser = deleter,
                            CurrentNum = thisCurrentNum,
                            PlaceTbId = placeid,
                            RoomTbId = dto.RoomTBID!.Value,
                            MaterialTbId = dto.MaterialTBID!.Value,
                            Note2 = $"{FacilityTB!.Name}설비의 {MaintenceHistoryTB.Name}건 [시스템]재입고",
                            MaintenenceHistoryTbId = MaintenceHistoryTB.Id,
                            MaintenenceMaterialTbId = NewUseMaintenenceMaterialTB.Id
                        };

                        context.StoreTbs.Add(NewStoreTB);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }

                       
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }


        /// <summary>
        /// 유지보수 내용 삭제
        /// </summary>
        /// <param name="maintanceid"></param>
        /// <param name="placeid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> deleteMaintenanceRecord(DeleteMaintanceDTO2 dto, int placeid, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int MaintanceID in dto.MaintanceID)
                    {
                        MaintenenceHistoryTb? MaintenenceTB = await context.MaintenenceHistoryTbs
                            .FirstOrDefaultAsync(m => m.Id == MaintanceID &&
                                                      m.DelYn != true);

                        if (MaintenenceTB is null)
                        {
                            await transaction.RollbackAsync();
                            return null; // 없음
                        }
                            
                        MaintenenceTB.DelDt = DateTime.Now;
                        MaintenenceTB.DelUser = deleter;
                        MaintenenceTB.DelYn = true;

                        context.MaintenenceHistoryTbs.Update(MaintenenceTB);
                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return false; // 실패
                        }


                        List<StoreTb>? StoreList = await context.StoreTbs.Where(m => m.DelYn != true &&
                                                                                     m.Inout == 0 &&
                                                                                     m.MaintenenceHistoryTbId == MaintenenceTB.Id)
                                                .ToListAsync();

                        // 출고내용이 없을수도 있으니
                        if (StoreList is not null && StoreList.Any()) 
                        {
                            foreach(StoreTb StoreTB in StoreList)
                            {
                                //StoreTB.
                                FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == MaintenenceTB.FacilityTbId && m.DelYn != true);
                                if (FacilityTB is null)
                                {
                                    await transaction.RollbackAsync();
                                    return null; // 없음
                                }

                                StoreTB.DelDt = DateTime.Now;
                                StoreTB.DelYn = true;
                                StoreTB.DelUser = deleter;
                                StoreTB.Note = dto.Note; // 비고
                                StoreTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenenceTB.Name}건 [시스템]삭제";
                                context.StoreTbs.Update(StoreTB);
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false; // 실패
                                }

                                UseMaintenenceMaterialTb? UseMaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs
                                    .FirstOrDefaultAsync(m => m.DelYn != true && 
                                                              m.Inout == 0);

                                if (UseMaintenenceMaterialTB is null)
                                    return null;

                                UseMaintenenceMaterialTB.DelDt = DateTime.Now;
                                UseMaintenenceMaterialTB.DelYn = true;
                                UseMaintenenceMaterialTB.DelUser = deleter;
                                UseMaintenenceMaterialTB.Note = dto.Note;
                                UseMaintenenceMaterialTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenenceTB.Name}건 [시스템]삭제";

                                context.UseMaintenenceMaterialTbs.Update(UseMaintenenceMaterialTB);
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if (!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }

                                InventoryTb InventoryTB = new InventoryTb
                                {
                                    Num = StoreTB.Num,
                                    UnitPrice = StoreTB.UnitPrice,
                                    CreateDt = DateTime.Now,
                                    CreateUser = deleter,
                                    UpdateDt = DateTime.Now,
                                    UpdateUser = deleter,
                                    PlaceTbId = placeid,
                                    RoomTbId = StoreTB.RoomTbId,
                                    MaterialTbId = StoreTB.MaterialTbId
                                };

                                context.InventoryTbs.Add(InventoryTB);
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if (!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }

                                // 현재개수 가져와야함.
                                int thisCurrentNum = await context.InventoryTbs
                                    .Where(m => m.DelYn != true &&
                                                m.MaterialTbId == StoreTB.MaterialTbId &&
                                                m.RoomTbId == StoreTB.RoomTbId &&
                                                m.PlaceTbId == placeid)
                                    .SumAsync(m => m.Num);

                                // 새로 재입고
                                UseMaintenenceMaterialTb NewUseMaintenenceMaterialTB = new UseMaintenenceMaterialTb
                                {
                                    Inout = 1,
                                    Num = UseMaintenenceMaterialTB.Num,
                                    Unitprice = UseMaintenenceMaterialTB.Unitprice,
                                    Totalprice = UseMaintenenceMaterialTB.Totalprice,
                                    CreateDt = DateTime.Now,
                                    CreateUser = deleter,
                                    UpdateDt = DateTime.Now,
                                    UpdateUser = deleter,
                                    PlaceTbId = placeid,
                                    RoomTbId = StoreTB.RoomTbId,
                                    MaterialTbId = StoreTB.MaterialTbId,
                                    Note2 = $"{FacilityTB!.Name}설비의 {MaintenenceTB.Name}건 [시스템]재입고",
                                    MaintenanceTbId = MaintenenceTB.Id
                                };

                                context.UseMaintenenceMaterialTbs.Add(NewUseMaintenenceMaterialTB);
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if (!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }

                                // 새로 재입고
                                StoreTb NewStoreTB = new StoreTb
                                {
                                    Inout = 1,
                                    Num = StoreTB.Num,
                                    UnitPrice = StoreTB.UnitPrice,
                                    TotalPrice = StoreTB.TotalPrice,
                                    InoutDate = DateTime.Now,
                                    CreateDt = DateTime.Now,
                                    CreateUser = deleter,
                                    UpdateDt = DateTime.Now,
                                    UpdateUser = deleter,
                                    CurrentNum = thisCurrentNum,
                                    PlaceTbId = placeid,
                                    RoomTbId = StoreTB.RoomTbId,
                                    MaterialTbId = StoreTB.MaterialTbId,
                                    Note2 = $"{FacilityTB!.Name}설비의 {MaintenenceTB.Name}건 [시스템]재입고",
                                    MaintenenceHistoryTbId = MaintenenceTB.Id,
                                    MaintenenceMaterialTbId = NewUseMaintenenceMaterialTB.Id
                                };


                                context.StoreTbs.Add(NewStoreTB);
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if (!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false; // 실패
                                }

                              
                            }
                        }
                    }

                    await transaction.CommitAsync();
                    return true; // 성공
                }
                catch(Exception ex)
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
                    HistoryModel.WorkDT = HistoryTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 일자 CreateDT 그냥
                    HistoryModel.HistoryTitle = HistoryTB.Name; // 이력 Name 그냥
                    HistoryModel.Type = HistoryTB.Type; // 작업구분 Type 그냥
                    HistoryModel.Worker = HistoryTB.Worker; // 작업자 Worker 그냥

                    List<StoreTb> StoreList = await context.StoreTbs
                        .Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id &&
                                    m.DelYn != true &&
                                    m.Inout == 0)
                        .ToListAsync();

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
                        HistoryModel.WorkDT = HistoryTB.Workdt.ToString("yyyy-MM-dd HH:mm:ss"); // 일자 CreateDT 그냥
                        HistoryModel.HistoryTitle = HistoryTB.Name; // 이력 Name 그냥
                        HistoryModel.Type = HistoryTB.Type; // 작업구분 Type 그냥
                        HistoryModel.Worker = HistoryTB.Worker; // 작업자 Worker 그냥

                        List<StoreTb> StoreList = await context.StoreTbs
                            .Where(m => m.MaintenenceHistoryTbId == HistoryTB.Id && 
                                        m.DelYn != true && 
                                        m.Inout == 0)
                            .ToListAsync();

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

        public async ValueTask<int?> UpdateMaintenanceUseRecord(UpdateMaintenanceMaterialDTO dto, int placeid, string updater)
        {
            bool UpdateResult = false;
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    UseMaintenenceMaterialTb? UseMaterialTB = await context.UseMaintenenceMaterialTbs
                                                                .FirstOrDefaultAsync(m => m.Id == dto.UseMaintanceID &&
                                                                m.MaterialTbId == dto.MaterialID &&
                                                                m.RoomTbId == dto.RoomID &&
                                                                m.PlaceTbId == placeid &&
                                                                m.DelYn != true);
                    if (UseMaterialTB is null)
                        return null;

                    if (dto.Num > UseMaterialTB.Num) // 추가출고가 이루어 져야함.
                    {
                        int TargetNumber = dto.Num - UseMaterialTB.Num; // 원래 DB에서 넘어온 DTO 수량 만큼을 빼면 추가 출고가 이루어 져야할 개수임.
                                                                        // 개수 검사하기 위한 로직
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, dto.RoomID, dto.MaterialID, TargetNumber);

                        if (InventoryList is not [_, ..])
                            return -1; /* 수량이 아에 없음 */

                        if (InventoryList.Sum(i => i.Num) < TargetNumber)
                        {
                            return -1; /* 수량이 부족함. */
                        }

                        List<InventoryTb> OutModel = new List<InventoryTb>();


                        int result = 0;

                        foreach (InventoryTb? inventory in InventoryList)
                        {
                            if (result <= TargetNumber)
                            {
                                OutModel.Add(inventory);
                                result += inventory.Num;
                                if (result == TargetNumber)
                                {
                                    break; // 반복문 종료
                                }
                            }
                            else
                                break; // 반복문 종료
                        }


                        // 출고 로직
                        if (OutModel is [_, ..])
                        {
                            if (result >= TargetNumber) // 출고개수가 충분할때만 동작
                            {
                                int checksum = 0;
                                int outresult = 0;
                                foreach (InventoryTb OutInventorytb in OutModel)
                                {
                                    outresult += OutInventorytb.Num;
                                    if (TargetNumber > outresult)
                                    {
                                        checksum += OutInventorytb.Num;
                                        OutInventorytb.Num -= OutInventorytb.Num;
                                        OutInventorytb.UpdateDt = DateTime.Now;
                                        OutInventorytb.UpdateUser = updater;

                                        if (OutInventorytb.Num == 0)
                                        {
                                            OutInventorytb.DelYn = true;
                                            OutInventorytb.DelDt = DateTime.Now;
                                            OutInventorytb.DelUser = updater;
                                        }

                                        context.Update(OutInventorytb);
                                    }
                                    else
                                    {
                                        checksum += TargetNumber - (outresult - OutInventorytb.Num);

                                        outresult -= TargetNumber;
                                        OutInventorytb.Num = outresult;
                                        OutInventorytb.UpdateDt = DateTime.Now;
                                        OutInventorytb.UpdateUser = updater;

                                        if (OutInventorytb.Num == 0)
                                        {
                                            OutInventorytb.DelYn = true;
                                            OutInventorytb.DelDt = DateTime.Now;
                                            OutInventorytb.DelUser = updater;
                                        }

                                        context.Update(OutInventorytb);
                                    }
                                }

                                if (checksum != TargetNumber)
                                {
                                    Console.WriteLine("결과가 다름!");
                                    await transaction.RollbackAsync();
                                    return 0;
                                }
                                UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!UpdateResult)
                                {
                                    await transaction.RollbackAsync();
                                    return 0;
                                }
                            }

                            // Inventory 테이블에서 해당 품목의 개수 Sum
                            int thisCurrentNum = context.InventoryTbs
                                .Where(m => m.DelYn != true &&
                                m.MaterialTbId == dto.MaterialID &&
                                m.RoomTbId == dto.RoomID &&
                                m.PlaceTbId == placeid)
                                .Sum(m => m.Num);

                            UseMaintenenceMaterialTb? MaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs
                                .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == dto.UseMaintanceID);

                            if(MaintenenceMaterialTB is null)
                            {
                                await transaction.RollbackAsync();
                                return 0;
                            }

                            MaintenenceMaterialTB.Num += TargetNumber;
                            MaintenenceMaterialTB.UpdateDt = DateTime.Now;
                            MaintenenceMaterialTB.UpdateUser = updater;
                            MaintenenceMaterialTB.Note = dto.Note;

                            context.UseMaintenenceMaterialTbs.Update(MaintenenceMaterialTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                return 0;
                            }

                            StoreTb store = new StoreTb();
                            store.InoutDate = DateTime.Now; // 현재날짜로 추가출고
                            store.Inout = 0; // 여기는 출고로직에 해당하는 부분임.
                            store.Num = TargetNumber; // 추가출고 개수 분량만.
                            store.UnitPrice = dto.UnitPrice; // 기존 단가
                            store.TotalPrice = dto.TotalPrice; // 총수량
                            store.CreateDt = DateTime.Now;
                            store.CreateUser = updater;
                            store.UpdateDt = DateTime.Now;
                            store.UpdateUser = updater;
                            store.RoomTbId = dto.RoomID; // 공간ID
                            store.MaterialTbId = dto.MaterialID; // 자재 ID
                            store.CurrentNum = thisCurrentNum; // 현재수량
                            store.Note = dto.Note; // 비고
                            store.PlaceTbId = placeid; // 사업장
                            store.MaintenenceHistoryTbId = dto.MaintanceID;
                            store.MaintenenceMaterialTbId = MaintenenceMaterialTB.Id;

                            await context.StoreTbs.AddAsync(store);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if(!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                return 0;
                            }
                        }

                        await transaction.CommitAsync();
                        return dto.MaintanceID;
                    }
                    else if (dto.Num < UseMaterialTB.Num) // 입고가 이루어져야함.
                    {
                        // 입고로직 ++
                        int TargetNumber = UseMaterialTB.Num - dto.Num; // 입고해야할 개수

                        InventoryTb InventoryTB = new InventoryTb();
                        InventoryTB.Num = TargetNumber; // 입고수량
                        InventoryTB.UnitPrice = dto.UnitPrice; // 단가
                        InventoryTB.CreateDt = DateTime.Now;
                        InventoryTB.CreateUser = updater;
                        InventoryTB.UpdateDt = DateTime.Now;
                        InventoryTB.UpdateUser = updater;
                        InventoryTB.PlaceTbId = placeid;
                        InventoryTB.RoomTbId = dto.RoomID;
                        InventoryTB.MaterialTbId = dto.MaterialID;
                        await context.InventoryTbs.AddAsync(InventoryTB);

                        int thisCurrentNum = await context.InventoryTbs
                            .Where(m => m.DelYn != true &&
                            m.MaterialTbId == dto.MaterialID &&
                            m.RoomTbId == dto.RoomID &&
                            m.PlaceTbId == placeid)
                            .SumAsync(m => m.Num);

                        UseMaintenenceMaterialTb? MaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == dto.UseMaintanceID);
                        if(MaintenenceMaterialTB is null)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }
                        
                        MaintenenceMaterialTB.Num = TargetNumber;
                        MaintenenceMaterialTB.UpdateDt = DateTime.Now;
                        MaintenenceMaterialTB.UpdateUser = updater;
                        MaintenenceMaterialTB.Note = dto.Note;

                        context.UseMaintenenceMaterialTbs.Update(MaintenenceMaterialTB);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

                        StoreTb store = new StoreTb();
                        store.InoutDate = DateTime.Now; // 현재날짜로 추가출고
                        store.Inout = 1; // 여기는 입고로직에 해당하는 부분임.
                        store.Num = TargetNumber; // 추가출고 개수 분량만.
                        store.UnitPrice = dto.UnitPrice; // 기존 단가
                        store.TotalPrice = dto.TotalPrice; // 총수량
                        store.CreateDt = DateTime.Now;
                        store.CreateUser = updater;
                        store.UpdateDt = DateTime.Now;
                        store.UpdateUser = updater;
                        store.RoomTbId = dto.RoomID; // 공간ID
                        store.MaterialTbId = dto.MaterialID; // 자재 ID
                        store.CurrentNum = thisCurrentNum; // 현재수량
                        store.Note = dto.Note; // 비고
                        store.PlaceTbId = placeid; // 사업장
                        store.MaintenenceHistoryTbId = dto.MaintanceID;
                        store.MaintenenceMaterialTbId = MaintenenceMaterialTB.Id;

                        await context.StoreTbs.AddAsync(store);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

                        await transaction.CommitAsync();
                        return dto.MaintanceID;
                    }
                    else // 같으면 그대로임.
                    {
                        // 아무것도 아님. 현상 그대로 유지
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
