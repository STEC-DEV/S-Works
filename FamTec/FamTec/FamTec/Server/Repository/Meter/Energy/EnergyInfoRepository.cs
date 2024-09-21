using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace FamTec.Server.Repository.Meter.Energy
{
    public class EnergyInfoRepository : IEnergyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public EnergyInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 일일 검침값 입력 
        /// - 해당년도 월별 합산으로 들어감.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<EnergyDayUsageTb?> AddAsync(EnergyDayUsageTb model)
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

                        await context.EnergyDayUsageTbs.AddAsync(model);

                        bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!AddResult)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        // MONTH TB 있는지 검사
                        EnergyMonthUsageTb? MonthTBInfo = await context.EnergyMonthUsageTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true &&
                            m.MeterItemId == model.MeterItemId &&
                            m.Year == model.MeterDt.Year);

                        // 월별
                        if (MonthTBInfo is null)
                        {
                            MonthTBInfo = new EnergyMonthUsageTb
                            {
                                TotalUsage = 0,
                                Year = model.MeterDt.Year,
                                Month = model.MeterDt.Month,
                                CreateDt = DateTime.Now,
                                CreateUser = model.CreateUser,
                                UpdateDt = DateTime.Now,
                                UpdateUser = model.CreateUser,
                                MeterItemId = model.MeterItemId
                            };

                            await context.EnergyMonthUsageTbs.AddAsync(MonthTBInfo);
                            AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!AddResult)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }
                        }

                        float amount_result = await context.EnergyDayUsageTbs
                            .Where(m => m.DelYn != true &&
                                        m.MeterItemId == model.MeterItemId &&
                                        m.MeterDt.Year == model.MeterDt.Year &&
                                        m.MeterDt.Month == model.MeterDt.Month)
                            .SumAsync(m => m.TotalAmount);

                        EnergyMonthUsageTb? MonthUsageTB = await context.EnergyMonthUsageTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true && m.Year == model.MeterDt.Year && m.Month == model.MeterDt.Month && m.MeterItemId == model.MeterItemId);

                        if (MonthUsageTB is null)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        MonthUsageTB.TotalUsage = amount_result;
                        context.EnergyMonthUsageTbs.Update(MonthTBInfo);

                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        await transaction.CommitAsync();
                        return model;
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });
        }

        /// <summary>
        /// 해당년-월 데이터 리스트 출력 - 월간 전체
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public async ValueTask<List<DayEnergyDTO>?> GetMonthList(DateTime SearchDate, int placeid)
        {
            try
            {
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                // 해당년도-월 의 1일과 마지막일을 구함.
                List<DateTime> allDates = Enumerable.Range(1, 30)
                                        .Select(day => new DateTime(Years, Month, day))
                                        .ToList();

                // MeterItemTable의 Where 조건에 맞는 모든 List 반환
                List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                    .Where(m => m.DelYn != true && 
                                m.PlaceTbId == placeid && 
                                m.ContractTbId != null)
                    .ToListAsync();

                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                // 그룹 만들 데이터 크로스 조인
                List<DayTotalEnergyDTO>? crossJoinResult = (from AllDateList in allDates
                                                            from AllMeterList in allMeterItems
                                                            join a in
                                                                (from EnergyList in context.EnergyDayUsageTbs
                                                                join MeterItemList in context.MeterItemTbs
                                                                on EnergyList.MeterItemId equals MeterItemList.MeterItemId
                                                                where context.MeterItemTbs.Any(mt => mt.PlaceTbId == placeid &&
                                                                                                     mt.MeterItemId == MeterItemList.MeterItemId)
                                                                group EnergyList by new
                                                                {
                                                                    EnergyList.MeterItemId,
                                                                    EnergyList.MeterDt.Date 
                                                                }into g
                                                                select new
                                                                {
                                                                    g.Key.MeterItemId,
                                                                    DT = g.Key.Date,
                                                                    TotalUseAmount = g.Sum(x => x.TotalAmount)
                                                                })
                                                            on new 
                                                            {
                                                                AllMeterList.MeterItemId,
                                                                DT = AllDateList 
                                                            }
                                                            equals new 
                                                            {
                                                                a.MeterItemId, 
                                                                a.DT 
                                                            } into gj
                                                            from sub in gj.DefaultIfEmpty()
                                                            join ContractTypeTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid)
                                                            on AllMeterList.ContractTbId equals ContractTypeTB.Id
                                                            select new DayTotalEnergyDTO
                                                            {
                                                                MaterItemId = AllMeterList.MeterItemId,
                                                                ContractName = ContractTypeTB.Name,
                                                                Name = AllMeterList.Name,
                                                                Date = AllDateList,
                                                                TotalUseAmount = sub?.TotalUseAmount ?? 0
                                                            })
                                                            .ToList();

                if (crossJoinResult is null || !crossJoinResult.Any())
                    return null;

                // 그룹화
                List<DayEnergyDTO>? groupedResult = crossJoinResult
                    .GroupBy(x => new { x.MaterItemId, x.Name, x.ContractName })
                    .Select(g => new DayEnergyDTO
                    {
                        MaterItemId = g.Key.MaterItemId,
                        ContractName = g.Key.ContractName,
                        Name = g.Key.Name,
                        TotalList = g.OrderBy(x => x.Date).ToList(),
                        MeterUseAmountSum = g.Sum(x => x.TotalUseAmount)
                    })
                    .OrderBy(x => x.MaterItemId)
                    .ToList();

                if (groupedResult is not null && groupedResult.Any())
                    return groupedResult;
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
        /// 해당년-월 데이터 선택된 검침기 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<DayEnergyDTO>?> GetMeterMonthList(DateTime SearchDate, List<int> MeterId, int placeid)
        {
            try
            {
                /*
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                // 해당년도-월 의 1일과 마지막일을 구함.
                List<DateTime> allDates = Enumerable.Range(1, 30)
                    .Select(day => new DateTime(Years, Month, day))
                    .ToList();


                // MeterItem의 Where조건에 맞는 List 반환
                List<MeterItemTb>? selectMeterItems = await context.MeterItemTbs
                    .Where(m => MeterId.Contains(m.MeterItemId) &&
                                         m.PlaceTbId == placeid &&
                                         m.ContractTbId != null)
                    .ToListAsync();

                if (selectMeterItems is null || !selectMeterItems.Any())
                    return null;

                // 그룹 만들 데이터 크로스 조인
                List<DayTotalEnergyDTO>? crossJoinResult = (from AllDateList in allDates
                                                            from AllMeterList in selectMeterItems
                                                            join a in
                                                                (from EnergyList in context.EnergyUsageTbs
                                                                 join MeterItemList in context.MeterItemTbs
                                                           on EnergyList.MeterItemId equals MeterItemList.MeterItemId
                                                                 where context.MeterItemTbs.Any(mt => mt.PlaceTbId == placeid &&
                                                                                                mt.MeterItemId == MeterItemList.MeterItemId)
                                                                 group EnergyList by new
                                                                 {
                                                                     EnergyList.MeterItemId,
                                                                     EnergyList.MeterDt.Date
                                                                 } into g
                                                                 select new
                                                                 {
                                                                     g.Key.MeterItemId,
                                                                     DT = g.Key.Date,
                                                                     TotalUseAmount = g.Sum(x => x.UseAmount)
                                                                 })
                                                            on new
                                                            {
                                                                AllMeterList.MeterItemId,
                                                                DT = AllDateList
                                                            } equals new
                                                            {
                                                                a.MeterItemId,
                                                                a.DT
                                                            } into gj
                                                            from sub in gj.DefaultIfEmpty()
                                                            join ContractTypeTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid)
                                                            on AllMeterList.ContractTbId equals ContractTypeTB.Id
                                                            select new DayTotalEnergyDTO
                                                            {
                                                                MaterItemId = AllMeterList.MeterItemId,
                                                                ContractName = ContractTypeTB.Name,
                                                                Name = AllMeterList.Name,
                                                                Date = AllDateList,
                                                                TotalUseAmount = sub?.TotalUseAmount ?? 0
                                                            })
                                                     .ToList();

                if (crossJoinResult is null || !crossJoinResult.Any())
                    return null;

                // 그룹화
                List<DayEnergyDTO>? groupedResult = crossJoinResult
                    .GroupBy(x => new { x.MaterItemId, x.Name, x.ContractName })
                    .Select(g => new DayEnergyDTO
                    {
                        MaterItemId = g.Key.MaterItemId,
                        ContractName = g.Key.ContractName,
                        Name = g.Key.Name,
                        TotalList = g.OrderBy(x => x.Date).ToList()
                    })
                    .OrderBy(x => x.MaterItemId)
                    .ToList();

                if (groupedResult is not null && groupedResult.Any())
                    return groupedResult;
                else
                    return null;
                */
                return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 년도별 통계
        /// </summary>
        /// <param name="year"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<YearsTotalEnergyDTO>> GetYearsList(int year, int placeid)
        {
            try
            {
                //List<MeterItemTb>? MeterList = await context.MeterItemTbs
                //    .Where(m => m.PlaceTbId == placeid && 
                //                m.DelYn != true)
                //    .ToListAsync();

                //if (MeterList is null || !MeterList.Any())
                //    return new List<YearsTotalEnergyDTO>();


                //List<EnergyMonthUsageTb>? MonthUsageList = await context.EnergyMonthUsageTbs
                //    .Where(m => MeterList.Select(m => m.MeterItemId).Contains(m.MeterItemId) && 
                //                m.Years == year)
                //    .ToListAsync();

                //if(MonthUsageList is null || !MonthUsageList.Any())
                //    return new List<YearsTotalEnergyDTO>();

                //List<YearsTotalEnergyDTO>? model = (from MonthUsageTB in MonthUsageList
                //                                   join MeterTB in context.MeterItemTbs.Where(m => m.DelYn != true)
                //                                   on MonthUsageTB.MeterItemId equals MeterTB.MeterItemId
                //                                   join ContractTypeTB in context.ContractTypeTbs.Where(m => m.DelYn != true)
                //                                   on MeterTB.ContractTbId equals ContractTypeTB.Id
                //                                   select new YearsTotalEnergyDTO
                //                                   {
                //                                       MeterID = MonthUsageTB.MeterItemId,
                //                                       ContractType = ContractTypeTB.Name,
                //                                       Name = MeterTB.Name,
                //                                       JAN = MonthUsageTB.Jan!.Value, /* 1월 */
                //                                       FEB = MonthUsageTB.Feb!.Value, /* 2월 */
                //                                       MAR = MonthUsageTB.Mar!.Value, /* 3월 */
                //                                       APR = MonthUsageTB.Apr!.Value, /* 4월 */
                //                                       MAY = MonthUsageTB.May!.Value, /* 5월 */
                //                                       JUN = MonthUsageTB.Jun!.Value, /* 6월 */
                //                                       JUL = MonthUsageTB.Jul!.Value, /* 7월 */
                //                                       AUG = MonthUsageTB.Aug!.Value, /* 8월 */
                //                                       SEP = MonthUsageTB.Sep!.Value, /* 9월 */
                //                                       OCT = MonthUsageTB.Oct!.Value, /* 10월 */
                //                                       NOV = MonthUsageTB.Nov!.Value, /* 11월 */
                //                                       DEC = MonthUsageTB.Dec!.Value /* 12월 */
                //                                   }).ToList();

                //return model;
                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 년도별 선택된 검침기에 대한 통계
        /// </summary>
        /// <param name="year"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<YearsTotalEnergyDTO>> GetMeterYearsList(int year, List<int> MeterId, int placeid)
        {
            try
            {
                //List<MeterItemTb>? MeterList = await context.MeterItemTbs
                //    .Where(m => MeterId.Contains(m.MeterItemId) && 
                //                         m.PlaceTbId == placeid && 
                //                         m.DelYn != true)
                //    .ToListAsync();

                //if (MeterList is null || !MeterList.Any())
                //    return new List<YearsTotalEnergyDTO>();


                //List<EnergyMonthUsageTb>? MonthUsageList = await context.EnergyMonthUsageTbs
                //    .Where(m => MeterList.Select(m => m.MeterItemId).Contains(m.MeterItemId) &&
                //                m.Years == year)
                //    .ToListAsync();

                //if (MonthUsageList is null || !MonthUsageList.Any())
                //    return new List<YearsTotalEnergyDTO>();

                //List<YearsTotalEnergyDTO>? model = (from MonthUsageTB in MonthUsageList
                //                                    join MeterTB in context.MeterItemTbs.Where(m => m.DelYn != true)
                //                                    on MonthUsageTB.MeterItemId equals MeterTB.MeterItemId
                //                                    join ContractTypeTB in context.ContractTypeTbs.Where(m => m.DelYn != true)
                //                                    on MeterTB.ContractTbId equals ContractTypeTB.Id
                //                                    select new YearsTotalEnergyDTO
                //                                    {
                //                                        MeterID = MonthUsageTB.MeterItemId,
                //                                        ContractType = ContractTypeTB.Name,
                //                                        Name = MeterTB.Name,
                //                                        JAN = MonthUsageTB.Jan!.Value, /* 1월 */
                //                                        FEB = MonthUsageTB.Feb!.Value, /* 2월 */
                //                                        MAR = MonthUsageTB.Mar!.Value, /* 3월 */
                //                                        APR = MonthUsageTB.Apr!.Value, /* 4월 */
                //                                        MAY = MonthUsageTB.May!.Value, /* 5월 */
                //                                        JUN = MonthUsageTB.Jun!.Value, /* 6월 */
                //                                        JUL = MonthUsageTB.Jul!.Value, /* 7월 */
                //                                        AUG = MonthUsageTB.Aug!.Value, /* 8월 */
                //                                        SEP = MonthUsageTB.Sep!.Value, /* 9월 */
                //                                        OCT = MonthUsageTB.Oct!.Value, /* 10월 */
                //                                        NOV = MonthUsageTB.Nov!.Value, /* 11월 */
                //                                        DEC = MonthUsageTB.Dec!.Value /* 12월 */
                //                                    }).ToList();

                //return model;
                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 출력 - 전체
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<DayEnergyDTO>> GetDaysList(DateTime StartDate, DateTime EndDate, int placeid)
        {
            try
            {
                //DateTime Search_startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day); // 시작일
                //DateTime Search_endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day); // 종료일

                //// 시작일 ~~ 종료일 까지 1일단위로 List에 담음.
                //List<DateTime> dateList = Enumerable.Range(0, (Search_endDate - Search_startDate).Days + 1)
                //                                    .Select(offset => Search_startDate.AddDays(offset))
                //                                    .ToList();

                //// MeterItemTable의 Where 조건에 맞는 모든 List 반환
                //List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                //    .Where(m => m.DelYn != true &&
                //                m.PlaceTbId == placeid &&
                //                m.ContractTbId != null)
                //    .ToListAsync();

                //if (allMeterItems is null || !allMeterItems.Any())
                //    return null;

                //// 그룹 만들 데이터 크로스 조인
                //List<DayTotalEnergyDTO>? crossJoinResult = (from AllDateList in dateList
                //                                            from AllMeterList in allMeterItems
                //                                            join a in
                //                                                (from EnergyList in context.EnergyUsageTbs
                //                                                 join MeterItemList in context.MeterItemTbs
                //                                           on EnergyList.MeterItemId equals MeterItemList.MeterItemId
                //                                                 where context.MeterItemTbs.Any(mt => mt.PlaceTbId == placeid &&
                //                                                                                mt.MeterItemId == MeterItemList.MeterItemId)
                //                                                 group EnergyList by new
                //                                                 {
                //                                                     EnergyList.MeterItemId,
                //                                                     EnergyList.MeterDt.Date
                //                                                 } into g
                //                                                 select new
                //                                                 {
                //                                                     g.Key.MeterItemId,
                //                                                     DT = g.Key.Date,
                //                                                     TotalUseAmount = g.Sum(x => x.UseAmount)
                //                                                 })
                //                                            on new
                //                                            {
                //                                                AllMeterList.MeterItemId,
                //                                                DT = AllDateList
                //                                            } equals new
                //                                            {
                //                                                a.MeterItemId,
                //                                                a.DT
                //                                            } into gj
                //                                            from sub in gj.DefaultIfEmpty()
                //                                            join ContractTypeTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid)
                //                                            on AllMeterList.ContractTbId equals ContractTypeTB.Id
                //                                            select new DayTotalEnergyDTO
                //                                            {
                //                                                MaterItemId = AllMeterList.MeterItemId,
                //                                                ContractName = ContractTypeTB.Name,
                //                                                Name = AllMeterList.Name,
                //                                                Date = AllDateList,
                //                                                TotalUseAmount = sub?.TotalUseAmount ?? 0
                //                                            })
                //                                            .ToList();

                //if (crossJoinResult is null || !crossJoinResult.Any())
                //    return null;

                //// 그룹화
                //List<DayEnergyDTO>? groupedResult = crossJoinResult
                //    .GroupBy(x => new { x.MaterItemId, x.Name, x.ContractName })
                //    .Select(g => new DayEnergyDTO
                //    {
                //        MaterItemId = g.Key.MaterItemId,
                //        ContractName = g.Key.ContractName,
                //        Name = g.Key.Name,
                //        TotalList = g.OrderBy(x => x.Date).ToList()
                //    })
                //    .OrderBy(x => x.MaterItemId)
                //    .ToList();

                //if (groupedResult is not null && groupedResult.Any())
                //    return groupedResult;
                //else
                //    return null;
                return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 선택된 검침기의 선택된 일자 사이의 데이터 리스트 출력 - 선택
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<DayEnergyDTO>> GetMeterDaysList(DateTime StartDate, DateTime EndDate, List<int> MeterId, int placeid)
        {
            try
            {
                //DateTime Search_startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day); // 시작일
                //DateTime Search_endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day); // 종료일

                //// 시작일 ~~ 종료일 까지 1일단위로 List에 담음.
                //List<DateTime> dateList = Enumerable.Range(0, (Search_endDate - Search_startDate).Days + 1)
                //                                    .Select(offset => Search_startDate.AddDays(offset))
                //                                    .ToList();

                //// MeterItemTable의 Where 조건에 맞는 모든 List 반환
                //List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                //    .Where(m => MeterId.Contains(m.MeterItemId) &&
                //                                m.DelYn != true &&
                //                         m.PlaceTbId == placeid &&
                //                        m.ContractTbId != null)
                //    .ToListAsync();

                //if (allMeterItems is null || !allMeterItems.Any())
                //    return null;

                //// 그룹 만들 데이터 크로스 조인
                //List<DayTotalEnergyDTO>? crossJoinResult = (from AllDateList in dateList
                //                                            from AllMeterList in allMeterItems
                //                                            join a in
                //                                                (from EnergyList in context.EnergyUsageTbs
                //                                                 join MeterItemList in context.MeterItemTbs
                //                                           on EnergyList.MeterItemId equals MeterItemList.MeterItemId
                //                                                 where context.MeterItemTbs.Any(mt => mt.PlaceTbId == placeid &&
                //                                                                                mt.MeterItemId == MeterItemList.MeterItemId)
                //                                                 group EnergyList by new
                //                                                 {
                //                                                     EnergyList.MeterItemId,
                //                                                     EnergyList.MeterDt.Date
                //                                                 } into g
                //                                                 select new
                //                                                 {
                //                                                     g.Key.MeterItemId,
                //                                                     DT = g.Key.Date,
                //                                                     TotalUseAmount = g.Sum(x => x.UseAmount)
                //                                                 })
                //                                            on new
                //                                            {
                //                                                AllMeterList.MeterItemId,
                //                                                DT = AllDateList
                //                                            } equals new
                //                                            {
                //                                                a.MeterItemId,
                //                                                a.DT
                //                                            } into gj
                //                                            from sub in gj.DefaultIfEmpty()
                //                                            join ContractTypeTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid)
                //                                            on AllMeterList.ContractTbId equals ContractTypeTB.Id
                //                                            select new DayTotalEnergyDTO
                //                                            {
                //                                                MaterItemId = AllMeterList.MeterItemId,
                //                                                ContractName = ContractTypeTB.Name,
                //                                                Name = AllMeterList.Name,
                //                                                Date = AllDateList,
                //                                                TotalUseAmount = sub?.TotalUseAmount ?? 0
                //                                            })
                //                                            .ToList();

                //if (crossJoinResult is null || !crossJoinResult.Any())
                //    return null;

                //// 그룹화
                //List<DayEnergyDTO>? groupedResult = crossJoinResult
                //    .GroupBy(x => new { x.MaterItemId, x.Name, x.ContractName })
                //    .Select(g => new DayEnergyDTO
                //    {
                //        MaterItemId = g.Key.MaterItemId,
                //        ContractName = g.Key.ContractName,
                //        Name = g.Key.Name,
                //        TotalList = g.OrderBy(x => x.Date).ToList()
                //    })
                //    .OrderBy(x => x.MaterItemId)
                //    .ToList();

                //if (groupedResult is not null && groupedResult.Any())
                //    return groupedResult;
                //else
                //    return null;
                return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사용량 비교
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<GetUseCompareDTO>> GetUseCompareList(DateTime SearchDate, int placeid)
        {
            try
            {
                int inputYear = SearchDate.Year;
                string? inputMonth = SearchDate.Month switch
                {
                    1 => "JAN",
                    2 => "FEB",
                    3 => "MAR",
                    4 => "APR",
                    5 => "MAY",
                    6 => "JUN",
                    7 => "JUL",
                    8 => "AUG",
                    9 => "SEP",
                    10 => "OCT",
                    11 => "NOV",
                    12 => "DEC",
                    _ => null
                };
                List<int> meterItemIds = new List<int> { 5, 6 };  // METER_ITEM_IDs to filter by

                // Fetch data for both METER_ITEM_IDs for the current and previous years
                /*
                List<EnergyUsageTb> currentYearData = context.EnergyMonthUsageTbs
                                                .Where(d => d.Years == inputYear && meterItemIds.Contains(d.MeterItemId))
                                                .ToList();

                var previousYearData = context.EnergyMonthUsageTbs
                                                 .Where(d => d.Years == inputYear - 1 && meterItemIds.Contains(d.MeterItemId))
                                                 .ToList();

                // Iterate through each METER_ITEM_ID and calculate the result
                foreach (var currentData in currentYearData)
                {
                    var previousData = previousYearData.FirstOrDefault(d => d.MeterItemId == currentData.MeterItemId);

                    // Calculate the result based on the input month
                    float? result = inputMonth switch
                    {
                        "JAN" => currentData.Jan - (previousData?.Dec ?? 0),
                        "FEB" => currentData.Feb - currentData.Jan,
                        "MAR" => currentData.Mar - currentData.Feb,
                        "APR" => currentData.Apr - currentData.Mar,
                        "MAY" => currentData.May - currentData.Apr,
                        "JUN" => currentData.Jun - currentData.May,
                        "JUL" => currentData.Jul - currentData.Jun,
                        "AUG" => currentData.Aug - currentData.Jul,
                        "SEP" => currentData.Sep - currentData.Aug,
                        "OCT" => currentData.Oct - currentData.Sep,
                        "NOV" => currentData.Nov - currentData.Oct,
                        "DEC" => currentData.Dec - currentData.Nov,
                        _ => null
                    };

                    // Fetch the current and previous year data

                if (currentYearData == null)
                {
                    Console.WriteLine("No data found for the input year.");
                    return new List<GetUseCompareDTO>();
                }
                */
                // Calculate the result based on the input month
                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
