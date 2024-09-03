using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Validation;
using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public async ValueTask<EnergyUsageTb?> AddAsync(EnergyUsageTb model)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    await context.EnergyUsageTbs.AddAsync(model);

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
                        m.Years == model.MeterDt.Year.ToString()); // <== 2024 현재날짜로

                    // 월별
                    if (MonthTBInfo is null)
                    {
                        MonthTBInfo = new EnergyMonthUsageTb
                        {
                            Jan = 0, // 1월
                            Feb = 0, // 2월
                            Mar = 0, // 3월
                            Apr = 0, // 4월
                            May = 0, // 5월
                            Jun = 0, // 6월
                            Jul = 0, // 7월
                            Aug = 0, // 8월
                            Sep = 0, // 9월
                            Oct = 0, // 10월
                            Nov = 0, // 11월
                            Dec = 0, // 12월
                            Years = model.MeterDt.Year.ToString(),
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

                    float amount_result  = await context.EnergyUsageTbs
                        .Where(m => m.DelYn != true && 
                                    m.MeterDt.Year == model.MeterDt.Year && 
                                    m.MeterDt.Month == model.MeterDt.Month)
                        .SumAsync(m => m.UseAmount);

                    switch (model.MeterDt.Month)
                    {
                        case 1:
                            MonthTBInfo.Jan += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 2:
                            MonthTBInfo.Feb += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 3:
                            MonthTBInfo.Mar += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 4:
                            MonthTBInfo.Apr += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 5:
                            MonthTBInfo.May += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 6:
                            MonthTBInfo.Jun += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 7:
                            MonthTBInfo.Jul += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 8:
                            MonthTBInfo.Aug += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 9:
                            MonthTBInfo.Sep += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 10:
                            MonthTBInfo.Oct += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 11:
                            MonthTBInfo.Nov += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                        case 12:
                            MonthTBInfo.Dec += amount_result;
                            MonthTBInfo.UpdateDt = DateTime.Now;
                            MonthTBInfo.UpdateUser = model.CreateUser;
                            break;
                    }

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
        }

        /// <summary>
        /// 해당년-월 데이터 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public async ValueTask<List<DayEnergyDTO>?> GetMonthList(DateTime SearchDate, int placeid)
        {
            try
            {
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                // Step 1: Generate all dates for September 2024
                var allDates = Enumerable.Range(1, 30)
                    .Select(day => new DateTime(Years, Month, day))
                    .ToList();

                // Step 2: Generate all distinct METER_ITEM_IDs that are relevant
                var allMeterItems = (from a in context.EnergyUsageTbs
                                     join m in context.MeterItemTbs on a.MeterItemId equals m.MeterItemId
                                     where context.MeterItemTbs.Any(mt => mt.PlaceTbId == placeid && mt.MeterItemId == m.MeterItemId)
                                     select new { a.MeterItemId, m.Name })
                                     .Distinct()
                                     .ToList();

                // Step 3: Create a cross join of all dates and meter items to ensure every combination is covered
                var crossJoinResult = (from d in allDates
                                       from m in allMeterItems
                                       join a in
                                           (from e in context.EnergyUsageTbs
                                            join mi in context.MeterItemTbs on e.MeterItemId equals mi.MeterItemId
                                            where context.MeterItemTbs.Any(mt => mt.PlaceTbId == 3 && mt.MeterItemId == mi.MeterItemId)
                                            group e by new { e.MeterItemId, e.MeterDt.Date } into g
                                            select new
                                            {
                                                g.Key.MeterItemId,
                                                DT = g.Key.Date,
                                                TotalUseAmount = g.Sum(x => x.UseAmount)
                                            })
                                       on new { m.MeterItemId, DT = d } equals new { a.MeterItemId, a.DT } into gj
                                       from sub in gj.DefaultIfEmpty()
                                       select new DayTotalEnergyDTO
                                       {
                                           MaterItemId = m.MeterItemId,
                                           Name = m.Name,
                                           Date = d,
                                           TotalUseAmount = sub?.TotalUseAmount ?? 0
                                       })
                                       .ToList();

                // Step 4: Group the results by MaterItemId
                var groupedResult = crossJoinResult
                    .GroupBy(x => new { x.MaterItemId, x.Name })
                    .Select(g => new DayEnergyDTO
                    {
                        MaterItemId = g.Key.MaterItemId,
                        Name = g.Key.Name,
                        TotalList = g.OrderBy(x => x.Date).ToList() // Order by date within each group
                    })
                    .OrderBy(x => x.MaterItemId)
                    .ToList();

                return null;

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
