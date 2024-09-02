using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Meter.Energy
{
    public class EnergyInfoRepository : IEnergyInfoRepository
    {
        private WorksContext context;
        private ILogService LogService;

        public EnergyInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 일일 검침값 입력
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
                        .FirstOrDefaultAsync(m => m.DelYn != true && m.MeterItemId == model.MeterItemId && m.Years.Equals(DateTime.Now.Year)); // <== 2024 현재날짜로

                    // 월별
                    if (MonthTBInfo is null)
                    {
                        EnergyMonthUsageTb MonthTB = new EnergyMonthUsageTb
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
                            CreateDt = DateTime.Now,
                            CreateUser = model.CreateUser,
                            UpdateDt = DateTime.Now,
                            UpdateUser = model.CreateUser,
                            MeterItemId = model.MeterItemId
                        };

                        await context.EnergyMonthUsageTbs.AddAsync(MonthTB);
                        AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!AddResult)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 오늘 입력했는지 검색
        ///     - 입력했으면 수정으로 해야함.
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public async ValueTask<EnergyUsageTb?> GetDayEnergy(DateTime SearchDate)
        {
            return null;
        }
    }
}
