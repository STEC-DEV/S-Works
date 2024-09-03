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
        public async ValueTask<List<DaysTotalEnergyDTO>?> GetMonthList(DateTime SearchDate, int placeid)
        {
            try
            {

                int year = SearchDate.Year; // Example year input
                int month = SearchDate.Month;   // Example month input

                // Step 1: Retrieve all MeterItemTb records for the given placeid
                List<MeterItemTb> MeterItemList = await context.MeterItemTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                    .ToListAsync();

                // Step 2: Extract MeterialItemIDs from the MeterItemList
                List<int> meterialItemIDs = MeterItemList.Select(m => m.MeterItemId).ToList();

                // Step 3: Retrieve all EnergyUsageTb records that match the extracted MeterialItemIDs and are within the specified year and month, and join with MeterTb
                var dailyUsageWithMeter = await context.EnergyUsageTbs
                    .Where(e => meterialItemIDs.Contains(e.MeterItemId) && e.MeterDt.Year == year && e.MeterDt.Month == month && e.DelYn != true)
                    .Join(
                        context.MeterItemTbs, // The table to join with
                        energyUsage => energyUsage.MeterItemId, // The foreign key in EnergyUsageTb
                        meter => meter.MeterItemId, // The primary key in MeterTb
                        (energyUsage, meter) => new
                        {
                            MeterialItemID = energyUsage.MeterItemId, // Renaming to avoid conflict
                            energyUsage.MeterDt,
                            energyUsage.UseAmount,
                            MeterId = meter.MeterItemId, // Renaming to avoid conflict if needed
                            MeterName = meter.Name, // Assuming MeterTb has a property MeterName
                            PlaceId = meter.PlaceTbId   // Assuming MeterTb has a property Location
                                                        // Add other properties from MeterTb that you want to include
                        }
                    )
                    .GroupBy(e => e.MeterDt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalUseAmount = g.Sum(x => x.UseAmount),
                        MeterDetails = g.Select(x => new {
                            x.MeterId,
                            x.MeterName,
                            x.PlaceId
                            // Add any other MeterTb properties you want to include in the output
                        }).FirstOrDefault() // Since we are grouping by date, use the first meter details
                    })
                    .ToListAsync();

                // Step 4: Generate a complete list of days for the specified month
                var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                    .Select(day => new DateTime(year, month, day));

                // Step 5: Merge with daily usage data to ensure all days are covered
                var result = daysInMonth.GroupJoin(
                    dailyUsageWithMeter,
                    date => date,
                    usage => usage.Date,
                    (date, usage) => new
                    {
                        Date = date,
                        TotalUseAmount = usage.Sum(x => x.TotalUseAmount), // Will be 0 if there's no matching data
                        MeterDetails = usage.Select(x => x.MeterDetails).FirstOrDefault() // Gets meter details if available
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                Console.WriteLine("Adsfasdf");
                return null;

                /*
                List<MeterItemTb> MeterItemList = await context.MeterItemTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync();



                


                int year = SearchDate.Year;
                int month = SearchDate.Month;

                var dailyUsage = (
                        from MeterTB in context.MeterItemTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                        join EnergyTB in context.EnergyUsageTbs
                        on MeterTB.MeterItemId equals EnergyTB.MeterItemId
                        )
                                

                // Get all records for the given year and month, grouped by day
                //var dailyUsage = await context.EnergyUsageTbs
                    //.Where(m => m.DelYn != true && m.MeterDt.Year == year && m.MeterDt.Month == month)
                    //.GroupBy(m => m.MeterDt.Date)
                    //.Select(g => new
                    //{
                        //Date = g.Key,
                        //TotalUseAmount = g.Sum(x => x.UseAmount)
                    //})
                    //.ToListAsync();

                // Generate a complete list of days for the specified month
                var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                    .Select(day => new DateTime(year, month, day));

                // Merge with daily usage data to ensure all days are covered
                List<DaysTotalEnergyDTO> result = daysInMonth.GroupJoin(
                    dailyUsage,
                    date => date,
                    usage => usage.Date,
                    (date, usage) => new DaysTotalEnergyDTO
                    {
                        Date = date,
                        TotalUseAmount = usage.Sum(x => x.TotalUseAmount) // Will be 0 if there's no matching data
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return result;
                */
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
