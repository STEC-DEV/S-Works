using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Diagnostics;

namespace FamTec.Server.Repository.Maintenence
{
    public class MaintanceRepository : IMaintanceRepository
    {
        private readonly WorksContext context;

        private IFileService FileService;
        private ILogService LogService;

        private DirectoryInfo? di;
        private string? MaintanceFileFolderPath;

        public MaintanceRepository(WorksContext _context,
            IFileService _fileservice,
            ILogService _logservice)
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
        public async ValueTask<MaintenenceHistoryTb?> GetMaintenanceInfo(int id)
        {
            try
            {
                MaintenenceHistoryTb? model = await context.MaintenenceHistoryTbs
                    .FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 유지보수 내용 상세보기
        /// </summary>
        /// <param name="MaintanceID"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
                    ImageName = !String.IsNullOrWhiteSpace(maintenenceTB.Image) ? maintenenceTB.Image : null,
                    Image = !string.IsNullOrWhiteSpace(maintenenceTB.Image) ? await FileService.GetImageFile($"{Common.FileServer}\\{placeid}\\Maintance", maintenenceTB.Image) : null
                };

                var result = await (from UseMaintenenceTB in context.UseMaintenenceMaterialTbs
                                    where UseMaintenenceTB.DelYn != true && UseMaintenenceTB.MaintenanceTbId == maintenenceTB.Id && UseMaintenenceTB.DelYn != true
                                    join RoomTB in context.RoomTbs on UseMaintenenceTB.RoomTbId equals RoomTB.Id
                                    join MaterialTB in context.MaterialTbs on UseMaintenenceTB.MaterialTbId equals MaterialTB.Id
                                    where RoomTB.DelYn != true && MaterialTB.DelYn != true
                                    select new
                                    {
                                        UseMaintenenceMaterial = UseMaintenenceTB,
                                        Room = RoomTB,
                                        Material = MaterialTB
                                    }).ToListAsync();

                foreach (var item in result)
                {
                    if (item.Material is null || item.Room is null)
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
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        string? NewFileName = files is not null ? FileService.SetNewFileName(id.ToString(), files) : String.Empty;

                        MaintenenceHistoryTb? MaintenenceHistory = await context.MaintenenceHistoryTbs
                            .FirstOrDefaultAsync(m => m.Id == id &&
                                                        m.DelYn != true);

                        if (MaintenenceHistory is null)
                            return (bool?)null;

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
                                /* 파일 넣기 */
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
                    catch (DbUpdateConcurrencyException ex)
                    {
                        /* 다른곳에서 해당 품목을 사용중입니다.*/
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(); // 트랜잭션 롤백
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });
        }

        /// <summary>
        /// 유지보수 사용자재 (외주작업)등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="userid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<FailResult?> AddOutSourcingMaintanceAsync(AddMaintenanceDTO dto, string creater, string userid, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif

                // -1 동시성 에러
                // -2 삭제된 데이터를 조회하고있음.
                // 0 출고개수가 부족
                // 0 >출고완료
                /* 실패 리스트 담을곳 */
                FailResult ReturnResult = new FailResult();

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                        MaintenenceHistory.Name = dto.Name!; // 작업명
                        MaintenenceHistory.Type = 1; // 외주작업
                        MaintenenceHistory.Worker = dto.Worker!; // 작업자
                        MaintenenceHistory.Workdt = dto.WorkDT; // 작업일자
                        MaintenenceHistory.TotalPrice = dto.TotalPrice!.Value; // 소요비용
                        MaintenenceHistory.CreateDt = DateTime.Now; // 생성일자
                        MaintenenceHistory.CreateUser = creater; // 작업자
                        MaintenenceHistory.UpdateDt = DateTime.Now; // 수정일자
                        MaintenenceHistory.UpdateUser = creater; // 수정자
                        MaintenenceHistory.FacilityTbId = dto.FacilityID!.Value; // 설비ID

                        await context.MaintenenceHistoryTbs.AddAsync(MaintenenceHistory);
                        bool AddHistoryResult = await context.SaveChangesAsync() > 0 ? true : false; // 저장
                        if (!AddHistoryResult)
                        {
                            // 동시성 에러
                            await transaction.RollbackAsync();
                            ReturnResult.ReturnResult = -1;
                            return ReturnResult;
                        }

                        // 유지보수 등록완료
                        await transaction.CommitAsync();
                        ReturnResult.ReturnResult = MaintenenceHistory.Id;
                        return ReturnResult;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        /* 다른 곳에서 해당 품목을 사용중입니다. */
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        ReturnResult.ReturnResult = -1;
                        return ReturnResult;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        ReturnResult.ReturnResult = -2;
                        return ReturnResult;
                    }
                }
            });
        }

        /// <summary>
        /// 유지보수 사용자재 (자체작업)등록 -- 출고등록과 비슷하다 보면됨.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FailResult?> AddSelfMaintanceAsync(AddMaintenanceDTO dto, string creater, string userid, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                // -1 동시성 에러
                // -2 삭제된 데이터를 조회하고있음.
                // 0 출고개수가 부족
                // 0 >출고완료

                /* 실패 리스트 담을곳 */
                FailResult ReturnResult = new FailResult();
                bool UpdateResult = false;

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        /* 수량 체크 */
                        foreach (InOutInventoryDTO inventoryItem in dto.Inventory!)
                        {
                            /* 출고할게 여러곳에 있으니 전체 Check 개수 Check */
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, inventoryItem.AddStore!.RoomID!.Value, inventoryItem.MaterialID!.Value, inventoryItem.AddStore.Num!.Value);

                            if (InventoryList is null || !InventoryList.Any())
                            {
                                MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => m.Id == inventoryItem.MaterialID && m.DelYn != true);
                                if (MaterialTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == inventoryItem.AddStore.RoomID && m.DelYn != true);
                                if (RoomTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                int avail_Num = await context.InventoryTbs
                                .Where(m => m.PlaceTbId == placeid &&
                                m.MaterialTbId == inventoryItem.MaterialID &&
                                m.RoomTbId == inventoryItem.AddStore.RoomID).SumAsync(m => m.Num);

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
                                if (InventoryList.Sum(i => i.Num) < inventoryItem.AddStore.Num)
                                {
                                    /* 수량이 부족함. */
                                    MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => m.Id == inventoryItem.MaterialID && m.DelYn != true);
                                    RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == inventoryItem.AddStore.RoomID && m.DelYn != true);

                                    if (MaterialTB is null || RoomTB is null)
                                    {
                                        ReturnResult.ReturnResult = -2;
                                        return ReturnResult;
                                    }

                                    int avail_Num = await context.InventoryTbs
                                    .Where(m => m.PlaceTbId == placeid &&
                                                m.MaterialTbId == inventoryItem.MaterialID &&
                                                m.RoomTbId == inventoryItem.AddStore.RoomID)
                                    .SumAsync(m => m.Num);

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

                        // 실패 List가 NULl OR COUNT = 0 이 아닌경우
                        if (ReturnResult.FailList is [_, ..])
                        {
                            ReturnResult.ReturnResult = 0;
                            return ReturnResult;
                        }

                        /* 유지보수 이력에 추가. -- 여기서 변경해도 동시성검사 걸림. */
                        MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                        MaintenenceHistory.Name = dto.Name!; /* 작업명 */
                        MaintenenceHistory.Type = 0; /* 작업구분 (자체작업 / 외주작업 ..) */
                        MaintenenceHistory.Worker = dto.Worker!; /* 작업자 */
                        MaintenenceHistory.Workdt = dto.WorkDT; // 작업일자
                        MaintenenceHistory.CreateDt = DateTime.Now; /* 생성일자 */
                        MaintenenceHistory.CreateUser = creater; /* 생성자 */
                        MaintenenceHistory.UpdateDt = DateTime.Now; /* 수정일자 */
                        MaintenenceHistory.UpdateUser = creater; /* 수정자 */
                        MaintenenceHistory.FacilityTbId = dto.FacilityID!.Value; /* 설비 ID */

                        await context.MaintenenceHistoryTbs.AddAsync(MaintenenceHistory);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false; /* 저장 */

                        if (!UpdateResult)
                        {
                            /* 동시성 에러 */
                            await transaction.RollbackAsync();

                            ReturnResult.ReturnResult = -1;
                            return ReturnResult;
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
                                        break; /* 반복문 종료 */
                                    }
                                }
                                else
                                    break; /* 반복문 종료 */
                            }


                            List<int> StoreID = new List<int>(); // 외래키 박기위해서 리스트에 담음.
                            float this_TotalPrice = 0;
                            if (OutModel is [_, ..])
                            {
                                /* 출고개수가 충분할때만 동작. */
                                if (result >= model.AddStore.Num)
                                {
                                    /* 넘어온 수량이랑 실제로 빠지는 수량이랑 같은지 검사하는 CheckSum */
                                    int checksum = 0;
                                    /* 개수만큼 - 빼주면 됨 */
                                    int outresult = 0;
                                    foreach (InventoryTb OutInventoryTb in OutModel)
                                    {
                                        outresult += OutInventoryTb.Num;
                                        if (model.AddStore.Num > outresult)
                                        {
                                            int OutStoreEA = OutInventoryTb.Num;
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
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();

                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            /* Inventory 테이블에서 해당 품목의 개수 Sum */
                                            int thisCurrentNum = context.InventoryTbs
                                                .Where(m => m.DelYn != true &&
                                                            m.MaterialTbId == model.MaterialID &&
                                                            m.RoomTbId == model.AddStore.RoomID &&
                                                            m.PlaceTbId == placeid)
                                                .Sum(m => m.Num);

                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0; // 출고
                                            StoreTB.Num = OutStoreEA;// 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총 금액
                                            StoreTB.InoutDate = model.AddStore.InOutDate;
                                            StoreTB.CreateDt = DateTime.Now;
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = DateTime.Now;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = model.AddStore.RoomID!.Value;
                                            StoreTB.MaterialTbId = model.MaterialID!.Value;
                                            StoreTB.CurrentNum = thisCurrentNum;
                                            StoreTB.Note = model.AddStore.Note;
                                            StoreTB.PlaceTbId = placeid;
                                            StoreTB.MaintenenceHistoryTbId = MaintenenceHistory.Id;

                                            await context.StoreTbs.AddAsync(StoreTB);
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();

                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }
                                            StoreID.Add(StoreTB.Id); // ID를 추가함 --> 해당 컬럼 검색후 UseMaintenenceMaterialTB 외래키 박아넣어야 하기 때문

                                            this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                        }
                                        else
                                        {
                                            int OutStoreEA = model.AddStore.Num!.Value - (outresult - OutInventoryTb.Num);

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
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();

                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            /* Inventory 테이블에서 해당 품목의 개수 Sum */
                                            int thisCurrentNum = context.InventoryTbs
                                                .Where(m => m.DelYn != true &&
                                                            m.MaterialTbId == model.MaterialID &&
                                                            m.RoomTbId == model.AddStore.RoomID &&
                                                            m.PlaceTbId == placeid)
                                                .Sum(m => m.Num);

                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0; // 출고
                                            StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                            StoreTB.InoutDate = model.AddStore.InOutDate;
                                            StoreTB.CreateDt = DateTime.Now;
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = DateTime.Now;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = model.AddStore.RoomID!.Value;
                                            StoreTB.MaterialTbId = model.MaterialID!.Value;
                                            StoreTB.CurrentNum = thisCurrentNum;
                                            StoreTB.Note = model.AddStore.Note;
                                            StoreTB.PlaceTbId = placeid;
                                            StoreTB.MaintenenceHistoryTbId = MaintenenceHistory.Id;

                                            await context.StoreTbs.AddAsync(StoreTB);

                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }
                                            StoreID.Add(StoreTB.Id);

                                            this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                        }
                                    }


                                    if (checksum != model.AddStore.Num)
                                    {
                                        /* 출고하고자 하는 개수와 실제 개수가 다름. (동시성에서 누가 먼저 뺏을경우 발생함.) */
                                        Console.WriteLine("결과가 다름 RollBack!");
                                        await transaction.RollbackAsync();
                                        ReturnResult.ReturnResult = -1;
                                        return ReturnResult;
                                    }

                                    await context.SaveChangesAsync();
                                }
                            }

                            // 여기가 먼저 나와야함.
                            UseMaintenenceMaterialTb MaintenenceMaterialTB = new UseMaintenenceMaterialTb();
                            MaintenenceMaterialTB.Num = model.AddStore.Num!.Value; // 총수량
                            MaintenenceMaterialTB.Totalprice = this_TotalPrice;  // 총금액
                            MaintenenceMaterialTB.MaterialTbId = model.MaterialID!.Value;
                            MaintenenceMaterialTB.RoomTbId = model.AddStore.RoomID!.Value;
                            MaintenenceMaterialTB.MaintenanceTbId = MaintenenceHistory.Id;
                            MaintenenceMaterialTB.Note = model.AddStore.Note;
                            MaintenenceMaterialTB.CreateDt = DateTime.Now;
                            MaintenenceMaterialTB.CreateUser = creater;
                            MaintenenceMaterialTB.UpdateDt = DateTime.Now;
                            MaintenenceMaterialTB.UpdateUser = creater;
                            MaintenenceMaterialTB.PlaceTbId = placeid;
                            await context.UseMaintenenceMaterialTbs.AddAsync(MaintenenceMaterialTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                /* 저장 실패 - (트랜잭션) */
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }

                            foreach (int ID in StoreID)
                            {
                                StoreTb? UpdateStoreInfo = await context.StoreTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == ID);
                                if (UpdateStoreInfo is null)
                                {
                                    await transaction.RollbackAsync();
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                UpdateStoreInfo.MaintenenceMaterialTbId = MaintenenceMaterialTB.Id;
                                context.Update(UpdateStoreInfo);
                            }

                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }

                            MaintenenceHistory.UpdateDt = DateTime.Now;
                            MaintenenceHistory.UpdateUser = creater;
                            MaintenenceHistory.TotalPrice += this_TotalPrice;
                            context.MaintenenceHistoryTbs.Update(MaintenenceHistory);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }

                        }

                        /* 여기까지 오면 출고완료 */
                        await transaction.CommitAsync();
                        ReturnResult.ReturnResult = MaintenenceHistory.Id;
                        /* 저장된 유지보수 테이블 ID를 반환한다. */
                        return ReturnResult;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        /* 다른곳에서 해당 품목을 사용중입니다.*/
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        ReturnResult.ReturnResult = -1;
                        return ReturnResult;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        ReturnResult.ReturnResult = -2;
                        return ReturnResult;
                    }
                }
            });
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
                    di = new DirectoryInfo(MaintanceFileFolderPath);
                    if (!di.Exists) di.Create();

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

                        List<UseMaintenenceMaterialTb> UseList = await context.UseMaintenenceMaterialTbs
                            .Where(m => m.MaintenanceTbId == HistoryTB.Id && m.DelYn != true).ToListAsync();


                        if (UseList is [_, ..]) // 유지보수 이력이면 무조껀 있어야함.
                        {
                            foreach (UseMaintenenceMaterialTb UseTB in UseList)
                            {
                                //RoomTb? RoomTB = await context.RoomTbs
                                //    .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == StoreTB.RoomTbId);

                                MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m =>
                                                                                        m.Id == UseTB.MaterialTbId &&
                                                                                        m.DelYn != true);

                                if (MaterialTB is not null)
                                {
                                    MaintanceModel.UsedMaterialList.Add(new UsedMaterialDTO
                                    {
                                        MaintanceId = UseTB.Id,
                                        RoomTBID = UseTB.RoomTbId,
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
        /// 유지보수용 출고내용 삭제 - 출고내용 삭제임 (금액 반영해야함)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> deleteMaintenanceStoreRecord(DeleteMaintanceDTO DeleteDTO, int placeid, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                bool UpdateResult = false;

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        MaintenenceHistoryTb? MaintanceHistoryTB = await context.MaintenenceHistoryTbs
                                .FirstOrDefaultAsync(m => m.Id == DeleteDTO.MaintanceID && m.DelYn != true);

                        if (MaintanceHistoryTB is null)
                            return (bool?)null;

                        FacilityTb? FacilityTB = await context.FacilityTbs
                        .FirstOrDefaultAsync(m => m.Id == MaintanceHistoryTB.FacilityTbId &&
                                                    m.DelYn != true);

                        if (FacilityTB is null)
                            return (bool?)null;

                        foreach (int UseMaintanceId in DeleteDTO.UseMaintenenceIDs)
                        {
                            UseMaintenenceMaterialTb? UseTB = await context.UseMaintenenceMaterialTbs
                            .FirstOrDefaultAsync(m => m.Id == UseMaintanceId && m.PlaceTbId == placeid && m.DelYn != true);

                            if (UseTB is null)
                                return (bool?)null;

                            UseTB.DelYn = true;
                            UseTB.DelUser = deleter;
                            UseTB.DelDt = DateTime.Now;

                            context.UseMaintenenceMaterialTbs.Update(UseTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                // 트랜잭션 걸림.
                                await transaction.RollbackAsync();
                                return false;
                            }

                            List<StoreTb>? StoreList = await context.StoreTbs
                                        .Where(m => m.MaintenenceMaterialTbId == UseTB.Id && m.DelYn != true && m.Inout == 0)
                                        .ToListAsync();

                            if (StoreList is [_, ..])
                            {
                                // 사용자재 1건에 대한 로그들 삭제 및 재입고
                                foreach (StoreTb StoreTB in StoreList)
                                {
                                    // 로그 삭제처리
                                    StoreTB.DelDt = DateTime.Now;
                                    StoreTB.DelYn = true;
                                    StoreTB.DelUser = deleter;
                                    StoreTB.Note = DeleteDTO.Note; // 삭제이유 있으면 삽입
                                    StoreTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintanceHistoryTB.Name}건 [시스템 삭제]";
                                    context.StoreTbs.Update(StoreTB);

                                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                    if (!UpdateResult)
                                    {
                                        // 트랜잭션 걸림.
                                        await transaction.RollbackAsync();
                                        return false;
                                    }

                                    // 삭제된 로그에 대해 재입고 처리
                                    InventoryTb NewInventoryTB = new InventoryTb();
                                    NewInventoryTB.Num = StoreTB.Num;
                                    NewInventoryTB.UnitPrice = StoreTB.UnitPrice;
                                    NewInventoryTB.CreateDt = DateTime.Now;
                                    NewInventoryTB.CreateUser = deleter;
                                    NewInventoryTB.UpdateDt = DateTime.Now;
                                    NewInventoryTB.UpdateUser = deleter;
                                    NewInventoryTB.PlaceTbId = placeid;
                                    NewInventoryTB.RoomTbId = StoreTB.RoomTbId;
                                    NewInventoryTB.MaterialTbId = StoreTB.MaterialTbId;

                                    context.InventoryTbs.Add(NewInventoryTB);
                                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                    if (!UpdateResult)
                                    {
                                        // 트랜잭션 걸림.
                                        await transaction.RollbackAsync();
                                        return false;
                                    }

                                    // 현재 개수 가져와야함.
                                    int thisCurrentNum = await context.InventoryTbs.Where(m => m.DelYn != true &&
                                                                                                m.MaterialTbId == StoreTB.MaterialTbId &&
                                                                                                m.RoomTbId == StoreTB.RoomTbId &&
                                                                                                m.PlaceTbId == placeid)
                                                                                    .SumAsync(m => m.Num);
                                    // 새로 재입고로그
                                    StoreTb NewStoreTB = new StoreTb();
                                    NewStoreTB.Inout = 1; // 입고
                                    NewStoreTB.Num = StoreTB.Num; // 출고된 수량만큼 입고
                                    NewStoreTB.UnitPrice = StoreTB.UnitPrice; // 출고된 단가로 입고
                                    NewStoreTB.TotalPrice = StoreTB.Num * StoreTB.UnitPrice;
                                    NewStoreTB.InoutDate = DateTime.Now; // 현재시간으로
                                    NewStoreTB.CreateDt = DateTime.Now;
                                    NewStoreTB.CreateUser = deleter;
                                    NewStoreTB.UpdateDt = DateTime.Now;
                                    NewStoreTB.UpdateUser = deleter;
                                    NewStoreTB.CurrentNum = StoreTB.Num + thisCurrentNum;
                                    NewStoreTB.PlaceTbId = placeid;
                                    NewStoreTB.RoomTbId = StoreTB.RoomTbId;
                                    NewStoreTB.MaterialTbId = StoreTB.RoomTbId;

                                    context.StoreTbs.Add(NewStoreTB);
                                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                    if (!UpdateResult)
                                    {
                                        await transaction.RollbackAsync();
                                        return false;
                                    }
                                }
                            }
                        }

                        // 유지보수 이력의 총금액 변경해야함.
                        List<UseMaintenenceMaterialTb>? UseMaterialList = await context.UseMaintenenceMaterialTbs
                            .Where(m => m.DelYn != true && m.MaintenanceTbId == DeleteDTO.MaintanceID).ToListAsync();

                        if (UseMaterialList is null)
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }

                        float totalPrice = UseMaterialList.Sum(m => m.Totalprice);

                        MaintanceHistoryTB.UpdateDt = DateTime.Now;
                        MaintanceHistoryTB.UpdateUser = deleter;
                        MaintanceHistoryTB.TotalPrice = totalPrice;

                        context.MaintenenceHistoryTbs.Update(MaintanceHistoryTB);
                        UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            // 다른곳에서 해당 품목을 사용중입니다.
                            await transaction.RollbackAsync();
                            return false;
                        }

                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // 다른곳에서 해당 품목을 사용중입니다.
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        // 에러 처리
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        return false;
                    }
                }

            });
        }


        /// <summary>
        /// 유지보수 자체를 삭제 - 금액변경 반영할 필요 X
        /// </summary>
        /// <param name="maintanceid"></param>
        /// <param name="placeid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> deleteMaintenanceRecord(DeleteMaintanceDTO2 dto, int placeid, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        bool UpdateResult = false;

                        foreach (int MaintanceID in dto.MaintanceID)
                        {
                            MaintenenceHistoryTb? MaintenenceTB = await context.MaintenenceHistoryTbs
                                .FirstOrDefaultAsync(m => m.Id == MaintanceID &&
                                                            m.DelYn != true);

                            if (MaintenenceTB is null)
                            {
                                await transaction.RollbackAsync();
                                return (bool?)null; // 없음
                            }

                            FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == MaintenenceTB.FacilityTbId && m.DelYn != true);
                            if (FacilityTB is null)
                                return null;

                            MaintenenceTB.DelDt = DateTime.Now;
                            MaintenenceTB.DelUser = deleter;
                            MaintenenceTB.DelYn = true;

                            context.MaintenenceHistoryTbs.Update(MaintenenceTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                return false; // 실패
                            }

                            List<UseMaintenenceMaterialTb> UseList = await context.UseMaintenenceMaterialTbs
                                .Where(m => m.MaintenanceTbId == MaintenenceTB.Id && m.DelYn != true).ToListAsync();

                            if (UseList is [_, ..])
                            {
                                foreach (UseMaintenenceMaterialTb UseTB in UseList)
                                {
                                    UseTB.DelYn = true;
                                    UseTB.DelDt = DateTime.Now;
                                    UseTB.DelUser = deleter;

                                    context.UseMaintenenceMaterialTbs.Update(UseTB);
                                    UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                    if (!UpdateResult)
                                    {
                                        await transaction.RollbackAsync();
                                        return false;
                                    }

                                    List<StoreTb>? StoreList = await context.StoreTbs
                                    .Where(m => m.DelYn != true &&
                                                m.MaintenenceMaterialTbId == UseTB.Id &&
                                                m.Inout == 0)
                                    .ToListAsync();

                                    if (StoreList is [_, ..])
                                    {
                                        foreach (StoreTb StoreTB in StoreList)
                                        {
                                            // 로그 삭제처리
                                            StoreTB.DelDt = DateTime.Now;
                                            StoreTB.DelYn = true;
                                            StoreTB.DelUser = deleter;
                                            StoreTB.InoutDate = DateTime.Now;
                                            StoreTB.Note = dto.Note;
                                            StoreTB.Note2 = $"{FacilityTB!.Name}설비의 {MaintenenceTB.Name}건 [시스템]삭제";
                                            context.StoreTbs.Update(StoreTB);


                                            int thisCurrentNum = await context.InventoryTbs
                                            .Where(m => m.DelYn != true &&
                                                m.MaterialTbId == StoreTB.MaterialTbId &&
                                                m.RoomTbId == StoreTB.RoomTbId &&
                                                m.PlaceTbId == StoreTB.PlaceTbId)
                                            .SumAsync(m => m.Num);

                                            StoreTb NewStoreTB = new StoreTb();
                                            NewStoreTB.Inout = 1; // 해당건 재 입고처리 로그
                                            NewStoreTB.Num = StoreTB.Num; // 해당 수량만큼
                                            NewStoreTB.UnitPrice = StoreTB.UnitPrice; // 해당 단가로
                                            NewStoreTB.TotalPrice = StoreTB.Num * StoreTB.UnitPrice; // 해당 금액으로
                                            NewStoreTB.InoutDate = DateTime.Now;
                                            NewStoreTB.CreateDt = DateTime.Now;
                                            NewStoreTB.CreateUser = deleter;
                                            NewStoreTB.UpdateDt = DateTime.Now;
                                            NewStoreTB.UpdateUser = deleter;
                                            NewStoreTB.CurrentNum = StoreTB.Num + thisCurrentNum;
                                            NewStoreTB.RoomTbId = StoreTB.RoomTbId;
                                            NewStoreTB.PlaceTbId = StoreTB.PlaceTbId;
                                            NewStoreTB.MaterialTbId = StoreTB.MaterialTbId;
                                            context.StoreTbs.Add(NewStoreTB);
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                return false;
                                            }

                                            InventoryTb NewInventoryTB = new InventoryTb()
                                            {
                                                Num = StoreTB.Num, // 출고된 수량만큼 입고
                                                UnitPrice = StoreTB.UnitPrice, // 출고된 수량의 단가
                                                CreateDt = DateTime.Now, // 현재 시간으로 입고
                                                CreateUser = deleter, // 해당 작업을 수행하는사람 이름으로 재입고
                                                UpdateDt = DateTime.Now,
                                                UpdateUser = deleter,
                                                PlaceTbId = StoreTB.PlaceTbId, // 해당 사업장
                                                RoomTbId = StoreTB.RoomTbId, // 해당 공간
                                                MaterialTbId = StoreTB.MaterialTbId // 해당 자재
                                            };

                                            context.InventoryTbs.Add(NewInventoryTB);
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        await transaction.CommitAsync();
                        return true; // 성공
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
            });
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
        public async ValueTask<List<MaintanceHistoryDTO>?> GetDateHistoryList(int placeid, DateTime StartDate, DateTime EndDate, List<string> Category, List<int> type)
        {
            try
            {
                List<BuildingTb>? BuildingTB = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .ToListAsync();

                if (BuildingTB is not [_, ..])
                    return null;

                List<FloorTb>? FloorTB = await context.FloorTbs
                    .Where(m => BuildingTB.Select(m => m.Id).Contains(m.BuildingTbId) 
                            && m.DelYn != true)
                    .ToListAsync();

                if (FloorTB is not [_, ..])
                    return null;

                List<RoomTb>? RoomTB = await context.RoomTbs
                    .Where(m => FloorTB.Select(m => m.Id).Contains(m.FloorTbId) 
                        && m.DelYn != true)
                    .ToListAsync();

                if (RoomTB is not [_, ..])
                    return null;

                // 설비유형
                List<FacilityTb>? FacilityList = await context.FacilityTbs
                    .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && 
                    m.DelYn != true &&
                    Category.Contains(m.Category))
                    .ToListAsync();

                // 작업구분 - 전체 or 일부
                List<MaintenenceHistoryTb>? HistoryList = await context.MaintenenceHistoryTbs
                    .Where(m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                    m.CreateDt >= StartDate &&
                    m.CreateDt <= EndDate.AddDays(1) &&
                    type.Contains(m.Type) && 
                    m.DelYn != true).ToListAsync();

                if (HistoryList is [_, ..])
                {
                    List<MaintanceHistoryDTO> HistoryDTO = new List<MaintanceHistoryDTO>();
                    // 반복문에서
                    // 설비유형 일자 이력 작업구분 작업자 사용자재 소요비용
                    foreach (MaintenenceHistoryTb HistoryTB in HistoryList)
                    {
                        MaintanceHistoryDTO HistoryModel = new MaintanceHistoryDTO();

                        FacilityTb? FacilityModel = await context.FacilityTbs
                            .FirstOrDefaultAsync(m => m.Id == HistoryTB.FacilityTbId &&
                                                m.DelYn != true);

                        HistoryModel.Category = FacilityModel!.Category; // 설비유형 --> FacilityTB 조회
                        HistoryModel.WorkDT = HistoryTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"); // 일자 CreateDT 그냥
                        HistoryModel.HistoryTitle = HistoryTB.Name; // 이력 Name 그냥
                        HistoryModel.Type = HistoryTB.Type; // 작업구분 Type 그냥
                        HistoryModel.Worker = HistoryTB.Worker; // 작업자 Worker 그냥


                        List<UseMaintenenceMaterialTb>? UseList = await context.UseMaintenenceMaterialTbs
                            .Where(m => m.MaintenanceTbId == HistoryTB.Id &&
                            m.DelYn != true).ToListAsync();

                        if (UseList is [_, ..])
                        {
                            foreach (UseMaintenenceMaterialTb UseTB in UseList)
                            {
                                MaterialTb? MaterialTB = await context.MaterialTbs
                                .FirstOrDefaultAsync(m => m.Id == UseTB.MaterialTbId);

                                HistoryModel.HistoryMaterialList.Add(new HistoryMaterialDTO
                                {
                                    MaterialID = MaterialTB!.Id,
                                    MaterialName = MaterialTB.Name
                                });
                            }

                            HistoryModel.TotalPrice = HistoryTB.TotalPrice; // 소요비용 - TotalPrice
                            HistoryDTO.Add(HistoryModel);
                        }
                    }
                    return HistoryDTO;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        public async ValueTask<List<AllMaintanceHistoryDTO>?> GetAllHistoryList(int placeid, List<string> Category, List<int> type)
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
                List<FacilityTb>? FacilityList = await context.FacilityTbs
                    .Where(m => RoomTB.Select(m => m.Id).Contains(m.RoomTbId) && 
                                m.DelYn != true &&
                                Category.Contains(m.Category))
                    .ToListAsync();

                // 작업구분
                List<MaintenenceHistoryTb>? HistoryList = await context.MaintenenceHistoryTbs
                    .Where(m => FacilityList.Select(m => m.Id).Contains(m.FacilityTbId) &&
                    m.DelYn != true &&
                    type.Contains(m.Type)).ToListAsync();


                if (HistoryList is [_, ..])
                {
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

                            List<UseMaintenenceMaterialTb>? UseList = await context.UseMaintenenceMaterialTbs
                                .Where(m => m.MaintenanceTbId == HistoryTB.Id &&
                                m.DelYn != true).ToListAsync();

                            if (UseList is [_, ..])
                            {
                                foreach (UseMaintenenceMaterialTb UseTB in UseList)
                                {
                                    // 사용자재 --> List<StoreTB> -- MaterialID빼서 Material foreach 사용자재 넣고    
                                    MaterialTb? MaterialTB = await context.MaterialTbs
                                        .FirstOrDefaultAsync(m => m.Id == UseTB.MaterialTbId);

                                    HistoryModel.HistoryMaterialList.Add(new HistoryMaterialDTO
                                    {
                                        MaterialID = MaterialTB!.Id,
                                        MaterialName = MaterialTB.Name
                                    });
                                }

                                HistoryModel.TotalPrice = HistoryTB.TotalPrice; // 소요비용 - TotalPrice
                                GroupItem.HistoryList.Add(HistoryModel);
                            }
                        }
                        AllMaintanceList.Add(GroupItem);
                    }
                    return AllMaintanceList;
                }
                else
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
        /// 유지보수 사용자재 추가 출고
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<FailResult?> AddMaintanceMaterialAsync(AddMaintanceMaterialDTO model, string creater, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                // -1 동시성 에러
                // -2 삭제된 데이터를 조회하고있음.
                // 0 출고개수가 부족
                // 0 >출고완료
                bool UpdateResult = false;

                /* 실패 리스트 담을곳 */
                FailResult ReturnResult = new FailResult();

                // 트랜잭션에 Serializable 격리 수준을 적용
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        // 유지보수 이력 Check
                        MaintenenceHistoryTb? MainteneceTB = await context.MaintenenceHistoryTbs.FirstOrDefaultAsync(m => m.Id == model.MaintanceID && m.DelYn != true);
                        if (MainteneceTB is null)
                        {
                            // 내용이 아예 없음.
                            await transaction.RollbackAsync();
                            ReturnResult.ReturnResult = -2;
                            return ReturnResult;
                        }

                        /* 수량체크 */
                        foreach (MaterialDTO MaterialInfo in model.MaterialList)
                        {
                            // 출고할게 여러곳에 있으니 전체 Check 개수 Check.
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, MaterialInfo.RoomID, MaterialInfo.MaterialID, MaterialInfo.Num);

                            if (InventoryList is null || !InventoryList.Any())
                            {
                                MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => m.Id == MaterialInfo.MaterialID && m.DelYn != true);
                                if (MaterialTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == MaterialInfo.RoomID && m.DelYn != true);
                                if (RoomTB is null)
                                {
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                int avail_Num = await context.InventoryTbs
                                .Where(m => m.PlaceTbId == placeid &&
                                            m.MaterialTbId == MaterialInfo.MaterialID &&
                                            m.RoomTbId == MaterialInfo.RoomID)
                                .SumAsync(m => m.Num);

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
                                if (InventoryList.Sum(i => i.Num) < MaterialInfo.Num)
                                {
                                    // 수량이 부족함.
                                    MaterialTb? MaterialTB = await context.MaterialTbs.FirstOrDefaultAsync(m => m.Id == MaterialInfo.MaterialID && m.DelYn != true);
                                    RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == MaterialInfo.RoomID && m.DelYn != true);

                                    if (MaterialTB is null || RoomTB is null)
                                    {
                                        ReturnResult.ReturnResult = -2;
                                        return ReturnResult;
                                    }

                                    int avail_Num = await context.InventoryTbs
                                    .Where(m => m.PlaceTbId == placeid &&
                                                m.MaterialTbId == MaterialInfo.MaterialID &&
                                                m.RoomTbId == MaterialInfo.RoomID)
                                    .SumAsync(m => m.Num);


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

                        // 실패 List가 NULl OR COUNT = 0 이 아닌경우
                        if (ReturnResult.FailList is [_, ..])
                        {
                            ReturnResult.ReturnResult = 0;
                            return ReturnResult;
                        }

                        /* 출고 로직시작 */
                        foreach (MaterialDTO MaterialInfo in model.MaterialList)
                        {
                            List<InventoryTb> OutModel = new List<InventoryTb>();

                            int? result = 0;

                            // 출고시킬 List를 만든다
                            List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, MaterialInfo.RoomID, MaterialInfo.MaterialID, MaterialInfo.Num);
                            if (InventoryList is not [_, ..])
                            {
                                /* 출고 개수가 부족함. */
                                ReturnResult.ReturnResult = 0;
                                return ReturnResult;
                            }

                            foreach (InventoryTb? inventory in InventoryList)
                            {
                                if (result <= MaterialInfo.Num)
                                {
                                    OutModel.Add(inventory);
                                    result += inventory.Num;
                                    if (result == MaterialInfo.Num)
                                    {
                                        break; /* 반복문 종료 */
                                    }
                                }
                                else
                                    break; /* 반복문 종료 */
                            }

                            List<int> StoreID = new List<int>(); // 외래키 박기위해서 리스트에 담음
                            float TotalPrice = 0; // 총액

                            if (OutModel is [_, ..])
                            {
                                /* 출고개수가 충분할때만 동작. */
                                if (result >= MaterialInfo.Num)
                                {
                                    /* 넘어온 수량이랑 실제로 빠지는 수량이랑 같은지 검사하는 CheckSum */
                                    int checksum = 0;

                                    /* 개수만큼 - 빼주면 됨 */
                                    int outresult = 0;

                                    foreach (InventoryTb OutInventoryTb in OutModel)
                                    {
                                        outresult += OutInventoryTb.Num;
                                        if (MaterialInfo.Num > outresult)
                                        {
                                            int OutStoreEA = OutInventoryTb.Num;
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
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            /* Inventory 테이블에서 해당 품목의 개수 Sum */
                                            int thisCurrentNum = context.InventoryTbs
                                            .Where(m => m.DelYn != true &&
                                                        m.MaterialTbId == MaterialInfo.MaterialID &&
                                                        m.RoomTbId == MaterialInfo.RoomID &&
                                                        m.PlaceTbId == placeid)
                                            .Sum(m => m.Num);

                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0; // 출고
                                            StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총 금액
                                            StoreTB.InoutDate = DateTime.Now; // 현재 시간으로 출고
                                            StoreTB.CreateDt = DateTime.Now;
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = DateTime.Now;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = MaterialInfo.RoomID;
                                            StoreTB.MaterialTbId = MaterialInfo.MaterialID;
                                            StoreTB.CurrentNum = thisCurrentNum;
                                            StoreTB.Note = MaterialInfo.Note;
                                            StoreTB.PlaceTbId = placeid;
                                            StoreTB.MaintenenceHistoryTbId = MainteneceTB.Id;

                                            await context.StoreTbs.AddAsync(StoreTB);
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }
                                            StoreID.Add(StoreTB.Id); // ID를 추가함 --> 해당컬럼 검색후 UseMaintenenceMaterialTB 외래키 박아야 하기 때문.

                                            TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액

                                        }
                                        else
                                        {
                                            int OutStoreEA = MaterialInfo.Num - (outresult - OutInventoryTb.Num);
                                            checksum += MaterialInfo.Num - (outresult - OutInventoryTb.Num);
                                            outresult -= MaterialInfo.Num;


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
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }

                                            // Inventory 테이블에서 해당 품목의 개수 Sum
                                            int thisCurrentNum = context.InventoryTbs
                                            .Where(m => m.DelYn != true &&
                                                        m.MaterialTbId == MaterialInfo.MaterialID &&
                                                        m.RoomTbId == MaterialInfo.RoomID &&
                                                        m.PlaceTbId == placeid)
                                            .Sum(m => m.Num);

                                            StoreTb StoreTB = new StoreTb();
                                            StoreTB.Inout = 0; // 출고
                                            StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                            StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 단가
                                            StoreTB.InoutDate = DateTime.Now; // 현재시간으로
                                            StoreTB.CreateDt = DateTime.Now; // 현재시간
                                            StoreTB.CreateUser = creater;
                                            StoreTB.UpdateDt = DateTime.Now;
                                            StoreTB.UpdateUser = creater;
                                            StoreTB.RoomTbId = MaterialInfo.RoomID;
                                            StoreTB.MaterialTbId = MaterialInfo.MaterialID;
                                            StoreTB.CurrentNum = thisCurrentNum;
                                            StoreTB.Note = MaterialInfo.Note;
                                            StoreTB.PlaceTbId = placeid;
                                            StoreTB.MaintenenceHistoryTbId = MainteneceTB.Id;

                                            await context.StoreTbs.AddAsync(StoreTB);
                                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                            if (!UpdateResult)
                                            {
                                                await transaction.RollbackAsync();
                                                ReturnResult.ReturnResult = -1;
                                                return ReturnResult;
                                            }
                                            StoreID.Add(StoreTB.Id);

                                            TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                        }
                                    }


                                    if (checksum != MaterialInfo.Num)
                                    {
                                        /* 출고하고자 하는 개수와 실제 개수가 다름. (동시성에서 누가 먼저 뺏을경우 발생함.) */
                                        Console.WriteLine("결과가 다름 RollBack!");
                                        await transaction.RollbackAsync();
                                        ReturnResult.ReturnResult = -1;
                                        return ReturnResult;
                                    }

                                    await context.SaveChangesAsync();
                                }
                            }


                            UseMaintenenceMaterialTb MaintenenceMaterialTB = new UseMaintenenceMaterialTb();

                            MaintenenceMaterialTB.Num = MaterialInfo.Num; // 수량
                            MaintenenceMaterialTB.Totalprice = TotalPrice; // 총  금액
                            MaintenenceMaterialTB.MaterialTbId = MaterialInfo.MaterialID;
                            MaintenenceMaterialTB.RoomTbId = MaterialInfo.RoomID;
                            MaintenenceMaterialTB.MaintenanceTbId = MainteneceTB.Id;
                            MaintenenceMaterialTB.Note = MaterialInfo.Note;
                            MaintenenceMaterialTB.CreateDt = DateTime.Now;
                            MaintenenceMaterialTB.CreateUser = creater;
                            MaintenenceMaterialTB.UpdateDt = DateTime.Now;
                            MaintenenceMaterialTB.UpdateUser = creater;
                            MaintenenceMaterialTB.PlaceTbId = placeid;

                            await context.UseMaintenenceMaterialTbs.AddAsync(MaintenenceMaterialTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                /* 저장 실패 - (트랜잭션) */
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }

                            foreach (int ID in StoreID)
                            {
                                StoreTb? UpdateStoreInfo = await context.StoreTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == ID);
                                if (UpdateStoreInfo is null)
                                {
                                    await transaction.RollbackAsync();
                                    ReturnResult.ReturnResult = -2;
                                    return ReturnResult;
                                }

                                UpdateStoreInfo.MaintenenceMaterialTbId = MaintenenceMaterialTB.Id;
                                context.Update(UpdateStoreInfo);
                            }

                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }


                            MainteneceTB.TotalPrice += TotalPrice;
                            MainteneceTB.UpdateDt = DateTime.Now;
                            MainteneceTB.UpdateUser = creater;
                            context.MaintenenceHistoryTbs.Update(MainteneceTB);
                            UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!UpdateResult)
                            {
                                await transaction.RollbackAsync();
                                ReturnResult.ReturnResult = -1;
                                return ReturnResult;
                            }
                        }

                        /* 여기까지 오면 출고완료 */
                        await transaction.CommitAsync();
                        /* 저장된 유지보수 테이블 ID를 반환한다. */

                        ReturnResult.ReturnResult = MainteneceTB.Id;
                        return ReturnResult;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        /* 다른곳에서 해당 품목을 사용중입니다.*/
                        await transaction.RollbackAsync();
                        LogService.LogMessage($"동시성 에러 {ex.Message}");
                        ReturnResult.ReturnResult = -1;
                        return ReturnResult;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
                        ReturnResult.ReturnResult = -2;
                        return ReturnResult;
                    }
                }
            });
        }

    }
}
