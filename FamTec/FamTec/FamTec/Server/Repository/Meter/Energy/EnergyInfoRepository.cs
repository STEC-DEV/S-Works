using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.EntityFrameworkCore;

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
                        m.Years == model.MeterDt.Year.ToString());

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
                                                                (from EnergyList in context.EnergyUsageTbs
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
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    }
}
