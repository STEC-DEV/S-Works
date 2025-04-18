﻿using FamTec.Server.Databases;
using FamTec.Server.Helpers;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FamTec.Server.Repository.Voc
{
    public class VocInfoRepository : IVocInfoRepository
    {
        private readonly WorksContext context;

        private IKakaoService KakaoService;
        private ILogService LogService;
        private ConsoleLogService<VocInfoRepository> CreateBuilderLogger;

        public VocInfoRepository(WorksContext _context,
            IKakaoService _kakaoservice,
            ILogService _logservice,
            ConsoleLogService<VocInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            this.KakaoService = _kakaoservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// VOC 엑셀 데이터 IMPORT
        /// </summary>
        /// <returns></returns>
        public async Task<int> ImportVocData(List<ConvertVocData> vocData)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음
                Debugger.Break();
#endif
                await using var transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false);

                try
                {
                    DateTime ThisDate = DateTime.Now;

                    foreach (var item in vocData)
                    {
                        // RandomCode 최대 재시도
                        const int MaxAttempts = 3;
                        int attempt = 0;

                        VocTb SaveResult = null!;

                        while(true)
                        {
                            attempt++;

                            var VocModel = new VocTb();
                            VocModel.Code = UniqueCodeHelpers.RandomCode();
                            VocModel.Title = item.title;
                            VocModel.Content = item.contents;
                            VocModel.Division = 1; // 밀어넣는건 무조껀 웹에서 해야함.
                            VocModel.Type = item.type;
                            VocModel.Phone = item.phone; // 전화번호
                            VocModel.Status = item.status;
                            VocModel.ReplyYn = false;
                            VocModel.CompleteDt = item.completeDT;
                            VocModel.CreateDt = item.createDT;
                            if (item.status == 2)
                            {
                                TimeSpan span = item.completeDT.Value - item.createDT;
                                VocModel.DurationDt = span.ToString();
                            }
                            VocModel.CreateUser = item.createUser; // 민원작성자
                            VocModel.UpdateDt = ThisDate;
                            VocModel.UpdateUser = item.updateUser; // 담당자
                            VocModel.BuildingTbId = item.buildingIdx; // 건물 ID

                            try
                            {
                                var AddResult = await context.VocTbs.AddAsync(VocModel).ConfigureAwait(false);
                                await context.SaveChangesAsync().ConfigureAwait(false);

                                // PK가 채워진 인스턴스를 보관
                                SaveResult = VocModel;
                                break;
                            }
                            catch (DbUpdateException ex) when (IsUniqueViolation(ex) && attempt < MaxAttempts)
                            {
                                // 중복 키 -> 트래킹 초기화 후 재시도
                                context.ChangeTracker.Clear();
                                continue;
                            }
                            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                            {
                                // 최대 재시도 초과
                                LogService.LogMessage(ex.ToString() + "접수번호 생성횟수 최대도달, 접수번호 생성에 실패했습니다.");
#if DEBUG
                                CreateBuilderLogger.ConsoleLog(ex);
#endif
                                return -1;
                            }  
                        }

                        if (item.status != 0)
                        {
                            var CommentModel = new CommentTb();
                            CommentModel.Content = item.completeContents;
                            CommentModel.Status = item.status;
                            CommentModel.CreateDt = item.completeDT!.Value; // 미처리가 아니면 완료시간이 있어야함.
                            CommentModel.CreateUser = item.updateUser; // createuser - 민원작성자 / updateuser - 담당자
                            CommentModel.UpdateDt = item.completeDT!.Value;
                            CommentModel.UpdateUser = item.updateUser; // 담당자
                            CommentModel.KakaosendYn = false;
                            CommentModel.VocTbId = SaveResult.Id;
                            CommentModel.UserTbId = item.userIdx; // 유저

                            var AddResult2 = await context.CommentTbs.AddAsync(CommentModel).ConfigureAwait(false);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return 1;
                }
                catch (Exception ex)
                {
                    // 예외시 롤백
                    await transaction.RollbackAsync().ConfigureAwait(false);

                    LogService.LogMessage(ex.ToString());
#if DEBUG
                    CreateBuilderLogger.ConsoleLog(ex);
#endif
                    return -2;
                }
                }
            );
        }

        /// <summary>
        /// MySQL Unique 제약 위반 검사
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is MySqlException mysql && mysql.Number == 1062)
                return true;
            if (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
                return true;
            return false;
        }

        /// <summary>
        /// DashBoard 용 금일 처리현황별 카운트
        /// </summary>
        /// <returns></returns>
        public async Task<VocDaysStatusCountDTO?> GetDashBoardDaysStatusData(DateTime NowDate,int placeid)
        {
            try
            {
                var BuildingList = await context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync();
                if (BuildingList is null)
                    return null;

                List<int> BuildingIdx = BuildingList.Select(m => m.Id).ToList();


                List<VocTb> VocList = await context.VocTbs
                .Where(m => m.DelYn != true && m.UpdateDt.Date == NowDate && BuildingIdx.Contains(m.BuildingTbId))
                .ToListAsync()
                .ConfigureAwait(false);

                // status 값에 따라 그룹화
                var groupedVocList = VocList
                    .GroupBy(m => m.Status) // Status 값으로 그룹화
                    .ToDictionary(g => g.Key, g => g.ToList()); // Dictionary 형태로 변환 (Key: status 값, Value: 해당 그룹의 목록)


                List<VocTb>? UnProcessList = groupedVocList.ContainsKey(0) ? groupedVocList[0] : new List<VocTb>();
                List<VocTb>? ProcessingList = groupedVocList.ContainsKey(1) ? groupedVocList[1] : new List<VocTb>();
                List<VocTb>? CompletedList = groupedVocList.ContainsKey(2) ? groupedVocList[2] : new List<VocTb>();

                VocDaysStatusCountDTO model = new VocDaysStatusCountDTO();
                model.UnProcessed = UnProcessList.Count();
                model.Processing = ProcessingList.Count();
                model.Completed = CompletedList.Count();
                model.Total = UnProcessList.Count() + ProcessingList.Count() + CompletedList.Count();

                return model;
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
        /// DashBoard 용 7일 처리현황별 카운트
        /// </summary>
        /// <returns></returns>
        public async Task<List<VocWeekStatusCountDTO>?> GetDashBoardWeeksStatusData(DateTime StartDate, DateTime EndDate, int placeid)
        {
            try
            {
                var allDates = Enumerable.Range(0, 1 + EndDate.Subtract(StartDate.AddDays(1)).Days)
                    .Select(offset => StartDate.AddDays(offset + 1).Date)
                    .ToList();

                var Buildinglist = await context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToListAsync();
                var BuildingIdx = Buildinglist.Select(m => m.Id).ToList();
                
                // Step 2: 날짜 범위 내의 데이터 가져오기
                var VocList = await context.VocTbs
                    .Where(m => m.DelYn != true && m.UpdateDt.Date >= StartDate.AddDays(1) && m.UpdateDt.Date <= EndDate && BuildingIdx.Contains(m.BuildingTbId))
                    .ToListAsync()
                    .ConfigureAwait(false);

                // Step 3: Status 값에 따라 그룹화
                var groupedVocList = VocList
                    .GroupBy(m => m.Status)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Step 4: 날짜별로 Status 카운트를 계산하여 모델 생성
                var model = allDates
                    .Select(date => new VocWeekStatusCountDTO
                    {
                        Date = date.ToString("MM.dd"),
                        UnProcessed = groupedVocList.ContainsKey(0)
                            ? groupedVocList[0].Count(v => v.UpdateDt.Date == date)
                            : 0,
                        Processing = groupedVocList.ContainsKey(1)
                            ? groupedVocList[1].Count(v => v.UpdateDt.Date == date)
                            : 0,
                        Completed = groupedVocList.ContainsKey(2)
                            ? groupedVocList[2].Count(v => v.UpdateDt.Date == date)
                            : 0,
                        Total = (groupedVocList.ContainsKey(0) ? groupedVocList[0].Count(v => v.UpdateDt.Date == date) : 0)
                          + (groupedVocList.ContainsKey(1) ? groupedVocList[1].Count(v => v.UpdateDt.Date == date) : 0)
                          + (groupedVocList.ContainsKey(2) ? groupedVocList[2].Count(v => v.UpdateDt.Date == date) : 0)
                    })
                    .ToList();

                // Step 5: 결과 반환
                return model;
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
        /// DashBoard 용 오늘 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        public async Task<VocDaysCountDTO?> GetDashBoardDaysData(DateTime NowDate, int placeid)
        {
            try
            {
                var Buildinglist = await context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToListAsync();
                var BuildingIdx = Buildinglist.Select(m => m.Id).ToList();

                // Step 1: 데이터베이스에서 오늘 날짜에 해당하는 데이터만 가져오기
                var groupedReceipts = await context.VocTbs
                    .Where(m => m.DelYn != true &&
                                BuildingIdx.Contains(m.BuildingTbId) &&
                                (m.Type >= 0 && m.Type <= 8) &&  // 0부터 8까지의 Type
                                m.CreateDt.Date == NowDate)       // 오늘 날짜만 필터링
                    .GroupBy(m => new { m.CreateDt.Date, m.Type }) // CreateDt의 Date와 Type을 기준으로 그룹화
                    .ToListAsync()
                    .ConfigureAwait(false);

                // Step 3: 단일 객체 생성 (오늘 날짜에 대한 데이터)
                var model = new VocDaysCountDTO
                {
                    DefaultType = groupedReceipts
                        .Where(g => g.Key.Type == 0)
                        .SelectMany(g => g)
                        .Count(),
                    MachineType = groupedReceipts
                        .Where(g => g.Key.Type == 1)
                        .SelectMany(g => g)
                        .Count(),
                    ElecType = groupedReceipts
                        .Where(g => g.Key.Type == 2)
                        .SelectMany(g => g)
                        .Count(),
                    liftType = groupedReceipts
                        .Where(g => g.Key.Type == 3)
                        .SelectMany(g => g)
                        .Count(),
                    ConstructType = groupedReceipts
                        .Where(g => g.Key.Type == 4)
                        .SelectMany(g => g)
                        .Count(),
                    FireType = groupedReceipts
                        .Where(g => g.Key.Type == 5)
                        .SelectMany(g => g)
                        .Count(),
                    NetWorkType = groupedReceipts
                        .Where(g => g.Key.Type == 6)
                        .SelectMany(g => g)
                        .Count(),
                    BeautyType = groupedReceipts
                        .Where(g => g.Key.Type == 7)
                        .SelectMany(g => g)
                        .Count(),
                    SecurityType = groupedReceipts
                        .Where(g => g.Key.Type == 8)
                        .SelectMany(g => g)
                        .Count()
                };
                model.TotalCount = model.DefaultType + model.MachineType + model.ElecType + model.liftType + model.ConstructType + model.FireType + model.NetWorkType + model.BeautyType + model.SecurityType;

                return model; // 단일 객체 반환
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
        /// DashBoard 용 일주일치 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        public async Task<List<VocWeekCountDTO>?> GetDashBoardWeeksData(DateTime LastDate, DateTime StartDate, int placeid)
        {
            try
            {
                // Step 1: 날짜 범위 생성
                var allDates = Enumerable.Range(0, 1 + LastDate.Subtract(StartDate.AddDays(1)).Days)
                         .Select(offset => StartDate.AddDays(offset + 1).Date)
                         .ToList();

                var adjustedEndDate = LastDate.Date.AddDays(1).AddTicks(-1);

                var BuildingList = await context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToListAsync();
                var BuildingIds = BuildingList.Select(m => m.Id);

                var groupedReceipts = await context.VocTbs
                    .Where(m => m.DelYn != true &&
                                (m.Type >= 0 && m.Type <= 8) &&
                                BuildingIds.Contains(m.BuildingTbId) &&
                                m.CreateDt >= StartDate.AddDays(1) && m.CreateDt <= adjustedEndDate)
                    .GroupBy(m => new { m.CreateDt.Date, m.Type })
                    .ToListAsync()
                    .ConfigureAwait(false);

                // Step 2: 데이터베이스에서 두 가지 유형의 데이터를 동시에 가져오기
                //var groupedReceipts = await context.VocTbs
                //    .Where(m => m.DelYn != true &&
                //                (m.Type == 0 || // 미분류
                //                m.Type == 1 ||  // 기계
                //                m.Type == 2 || // 전기
                //                m.Type == 3 || // 승강
                //                m.Type == 4 || // 소방
                //                m.Type == 5 || // 건축
                //                m.Type == 6 || // 통신
                //                m.Type == 7 || // 미화
                //                m.Type == 8) &&  // 보안
                //                m.CreateDt >= EndDate && m.CreateDt <= StartDate.AddTicks(-1))
                //    .GroupBy(m => new { m.CreateDt.Date, m.Type }) // CreateDt의 Date와 Type을 기준으로 그룹화
                //    .ToListAsync()
                //    .ConfigureAwait(false);

                // Step 3: 날짜별 데이터와 모든 날짜를 조인하여 결과 구성
                List<VocWeekCountDTO> model = allDates
                    .Select(date => new VocWeekCountDTO
                    {
                        Date = date.ToString("MM.dd"),
                        DefaultType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 0)
                            .SelectMany(g => g)
                            .Count(),
                        MachineType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 1)
                            .SelectMany(g => g)
                            .Count(),
                        ElecType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 2)
                            .SelectMany(g => g)
                            .Count(),
                        liftType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 3)
                            .SelectMany(g => g)
                            .Count(),
                        ConstructType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 4)
                            .SelectMany(g => g)
                            .Count(),
                        FireType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 5)
                            .SelectMany(g => g)
                            .Count(),
                        NetWorkType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 6)
                            .SelectMany(g => g)
                            .Count(),
                        BeautyType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 7)
                            .SelectMany(g => g)
                            .Count(),
                        SecurityType = groupedReceipts
                            .Where(g => g.Key.Date == date && g.Key.Type == 8)
                            .SelectMany(g => g)
                            .Count()
                    }).ToList();

                return model;
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
        /// VOC 추가
        /// </summary>
        /// <returns></returns>
        public async Task<VocTb?> AddAsync(VocTb model)
        {
            try
            {
                await context.VocTbs.AddAsync(model);
             
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
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
        /// 사업장별 민원 리스트 조회
        /// </summary>
        /// <returns></returns>
        public async Task<List<AllVocListDTO>?> GetVocList(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs.Where(m => type.Contains(m.Type) &&
                    status.Contains(m.Status) && 
                    division.Contains(m.Division!.Value) &&
                    buildingid.Contains(m.BuildingTbId))
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if(VocList is [_, ..])
                {
                    var grouplist = VocList.GroupBy(voc => new
                    {
                        voc.CreateDt.Year,
                        voc.CreateDt.Month
                    }).Select(group => new
                    {
                        Year = group.Key.Year,
                        Month = group.Key.Month,
                        VocList = group.ToList()
                    }).ToList();

                    if (grouplist is not [_, ..])
                        return null;

                    List<AllVocListDTO> AllVocList = new List<AllVocListDTO>();
                    foreach(var Group in grouplist)
                    {
                        AllVocListDTO VocItem = new AllVocListDTO();
                        VocItem.Years = Group.Year;
                        VocItem.Month = Group.Month;
                        VocItem.Dates = $"{Group.Year}-{Group.Month}";

                        List<VocListDTO> dto = (from VocTB in Group.VocList
                                                join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                                on VocTB.BuildingTbId equals BuildingTB.Id
                                                select new VocListDTO
                                                {
                                                    Id = VocTB.Id, // VOCID
                                                    BuildingName = BuildingTB.Name, // 건물명
                                                    Type = VocTB.Type, // 유형
                                                    Division = VocTB.Division,
                                                    Title = VocTB.Title, // 제목
                                                    Status = VocTB.Status, // 처리상태
                                                    CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 민원 요청시간
                                                    CreateUser = VocTB.CreateUser.ToString(), // 민원작성자
                                                    Phone = VocTB.Phone,
                                                    CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 민원처리 완료시간 -- .ToString() 에러
                                                    DurationDT = VocTB.DurationDt // 민원처리 소요시간
                                                }).OrderByDescending(m => m.CreateDT)
                                                .ToList();

                        VocItem.VocList = dto;
                        AllVocList.Add(VocItem);
                    }

                    if (AllVocList is [_, ..])
                        return AllVocList;
                    else
                        return null;
                }
                else
                {
                    return null;
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
        /// 월간 사업장별 VOC 조회 - V2
        /// </summary>
        /// <returns></returns>
        public async Task<List<VocListDTOV2>?> GetVocMonthListV2(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division, int searchyear, int searchmonth)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => type.Contains(m.Type) &&
                    status.Contains(m.Status) &&
                    division.Contains(m.Division!.Value) &&
                    buildingid.Contains(m.BuildingTbId) &&
                    m.CreateDt.Year == searchyear &&
                    m.CreateDt.Month == searchmonth)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync().ConfigureAwait(false);

                if (VocList is [_, ..])
                {
                    List<VocListDTOV2>? dto = (from VocTB in VocList
                                             join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                             on VocTB.BuildingTbId equals BuildingTB.Id
                                             select new VocListDTOV2
                                             {
                                                 createDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                 Id = VocTB.Id, // VOC ID
                                                 buildingName = BuildingTB.Name, // 건물명
                                                 type = VocTB.Type, // 유형
                                                 division = VocTB.Division!.Value, // 모바일 / 웹
                                                 title = VocTB.Title, // 민원제목
                                                 status = VocTB.Status, // 민원상태
                                                 completeDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                 createUser = VocTB.CreateUser.ToString(), // 작성자
                                                 phone = VocTB.Phone,
                                                 durationDT = VocTB.DurationDt, // 소요시간
                                                 replyYn = VocTB.ReplyYn,
                                             }).OrderByDescending(m => m.createDT)
                        .ToList();

                    return dto;
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
        /// 월간 사업장별 VOC 조회 [Regacy]
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <param name="division"></param>
        /// <param name="searchyear"></param>
        /// <param name="searchmonth"></param>
        /// <returns></returns>
        public async Task<List<VocListDTO>?> GetVocMonthList(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division, int searchyear, int searchmonth)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => type.Contains(m.Type) &&
                    status.Contains(m.Status) &&
                    division.Contains(m.Division!.Value) &&
                    buildingid.Contains(m.BuildingTbId) &&
                    m.CreateDt.Year == searchyear &&
                    m.CreateDt.Month == searchmonth)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync().ConfigureAwait(false);

                if(VocList is [_, ..])
                {
                    List<VocListDTO>? dto = (from VocTB in VocList
                                             join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                             on VocTB.BuildingTbId equals BuildingTB.Id
                                             select new VocListDTO
                                             {
                                                 CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                 Id = VocTB.Id, // VOC ID
                                                 BuildingName = BuildingTB.Name, // 건물명
                                                 Type = VocTB.Type, // 유형
                                                 Division = VocTB.Division, // 모바일 / 웹
                                                 Title = VocTB.Title, // 민원제목
                                                 Status = VocTB.Status, // 민원상태
                                                 CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                 CreateUser = VocTB.CreateUser.ToString(), // 작성자
                                                 Phone = VocTB.Phone,
                                                 DurationDT = VocTB.DurationDt // 소요시간
                                             }).OrderByDescending(m => m.CreateDT)
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 조건별 민원 리스트 조회 - V2
        /// </summary>
        /// <returns></returns>
        public async Task<List<VocListDTOV2>?> GetVocFilterListV2(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status, List<int> BuildingId, List<int> division)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => Type.Contains(m.Type) &&
                    status.Contains(m.Status) &&
                    division.Contains(m.Division!.Value) &&
                    BuildingId.Contains(m.BuildingTbId) &&
                    m.CreateDt >= StartDate && m.CreateDt <= EndDate.AddDays(1))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (VocList is [_, ..])
                {
                    List<VocListDTOV2>? dto = (from VocTB in VocList
                                             join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                             on VocTB.BuildingTbId equals BuildingTB.Id
                                             select new VocListDTOV2
                                             {
                                                 createDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                 Id = VocTB.Id, // VOC ID
                                                 buildingName = BuildingTB.Name, // 건물명
                                                 type = VocTB.Type, // 유형
                                                 division = VocTB.Division!.Value, // 모바일 / 웹
                                                 title = VocTB.Title, // 민원제목
                                                 status = VocTB.Status, // 민원상태
                                                 completeDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                 createUser = VocTB.CreateUser.ToString(), // 작성자
                                                 phone = VocTB.Phone,
                                                 durationDT = VocTB.DurationDt, // 소요시간
                                                 replyYn = VocTB.ReplyYn,
                                             }).OrderByDescending(m => m.createDT)
                                            .ToList();

                    return dto;
                }
                else
                {
                    // VocList 조회결과가 없음.
                    return null;
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
        /// 조건별 민원 리스트 조회 [Regacy]
        /// </summary>
        /// <returns></returns>
        public async Task<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status,List<int> BuildingID, List<int> division)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => Type.Contains(m.Type) &&
                    status.Contains(m.Status) &&
                    division.Contains(m.Division!.Value) &&
                    BuildingID.Contains(m.BuildingTbId) &&
                    m.CreateDt >= StartDate && m.CreateDt <= EndDate.AddDays(1))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (VocList is [_, ..])
                {
                    List<VocListDTO>? dto = (from VocTB in VocList
                                            join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                            on VocTB.BuildingTbId equals BuildingTB.Id
                                            select new VocListDTO
                                            {
                                                CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                Id = VocTB.Id, // VOC ID
                                                BuildingName = BuildingTB.Name, // 건물명
                                                Type = VocTB.Type, // 유형
                                                Division = VocTB.Division, // 모바일 / 웹
                                                Title = VocTB.Title, // 민원제목
                                                Status = VocTB.Status, // 민원상태
                                                CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                CreateUser = VocTB.CreateUser.ToString(), // 작성자
                                                Phone = VocTB.Phone,
                                                DurationDT = VocTB.DurationDt // 소요시간
                                            }).OrderByDescending(m => m.CreateDT)
                                            .ToList();

                    return dto;
                }
                else
                {
                    // VocList 조회결과가 없음.
                    return null;
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
        /// ID로 민원 상세조회
        /// </summary>
        /// <returns></returns>
        public async Task<VocTb?> GetVocInfoById(int vocid)
        {
            try
            {
                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Id == vocid && m.DelYn != true)
                    .ConfigureAwait(false);

                if(model is not null)
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
        /// Code로 민원 상세조회
        /// </summary>
        /// <returns></returns>
        public async Task<VocTb?> GetVocInfoByCode(string code)
        {
            try
            {
                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Code == code && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
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

        public async Task<bool> UpdateVocInfo(VocTb model)
        {
            try
            {
                context.VocTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
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

       
    }
}
