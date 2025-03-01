﻿using FamTec.Server.Databases;
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
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<EnergyInfoRepository> CreateBuilderLogger;

        public EnergyInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<EnergyInfoRepository> _createbuilderlogger
        )
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 해당 년-월-일 값 뽑아오기
        ///     - 어떻게 쓰일진 모르겠지만 아직까진 해당날짜에 데이터가 있는지 여부
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<EnergyDayUsageTb?> GetUsageDaysInfo(int meterid, int year, int month, int day, int placeid)
        {
            try
            {
                EnergyDayUsageTb? EnergyUseTB = await context.EnergyDayUsageTbs
                    .FirstOrDefaultAsync(m => m.MeterItemId == meterid &&
                                              m.Year == year &&
                                              m.Month == month &&
                                              m.Days == day &&
                                              m.PlaceTbId == placeid &&
                                              m.DelYn != true)
                    .ConfigureAwait(false);

                if (EnergyUseTB is not null)
                    return EnergyUseTB;
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

        /// <summary>
        /// 일일 검침값 입력 
        /// - 해당년도 월별 합산으로 들어감.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<EnergyDayUsageTb?> AddAsync(EnergyDayUsageTb model)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool SaveResult = false;

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
                        // 일별 데이터 저장
                        await context.EnergyDayUsageTbs.AddAsync(model);
                        SaveResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!SaveResult)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        // MONTH TB 있는지 검사
                        EnergyMonthUsageTb? MonthTBInfo = await context.EnergyMonthUsageTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true &&
                            m.MeterItemId == model.MeterItemId &&
                            m.Year == model.MeterDt.Year &&
                            m.Month == model.MeterDt.Month && 
                            m.PlaceTbId == model.PlaceTbId)
                            .ConfigureAwait(false);

                        // 일별데이터 저장 후 월별데이터를 저장해줘야함.
                        // 월별 데이터가 없다 -- 처음
                        if (MonthTBInfo is null)
                        {
                            MonthTBInfo = new EnergyMonthUsageTb
                            {
                                TotalUsage = model.TotalAmount,
                                Year = model.MeterDt.Year,
                                Month = model.MeterDt.Month,
                                CreateDt = DateTime.Now,
                                CreateUser = model.CreateUser,
                                UpdateDt = DateTime.Now,
                                UpdateUser = model.CreateUser,
                                MeterItemId = model.MeterItemId,
                                PlaceTbId = model.PlaceTbId
                            };

                            await context.EnergyMonthUsageTbs.AddAsync(MonthTBInfo).ConfigureAwait(false);
                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if (SaveResult)
                            {
                                await transaction.CommitAsync().ConfigureAwait(false);
                                return model;
                            }
                            else
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return null;
                            }
                        }
                        else // 월별 데이터가 있다 --> 월별 합산 해줘야함.
                        {
                            float amount_result = await context.EnergyDayUsageTbs
                            .Where(m => m.DelYn != true &&
                                        m.MeterItemId == model.MeterItemId &&
                                        m.MeterDt.Year == model.MeterDt.Year &&
                                        m.MeterDt.Month == model.MeterDt.Month && 
                                        m.PlaceTbId == model.PlaceTbId)
                            .SumAsync(m => m.TotalAmount)
                            .ConfigureAwait(false);

                            MonthTBInfo.TotalUsage = amount_result;
                            context.EnergyMonthUsageTbs.Update(MonthTBInfo);
                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if(SaveResult)
                            {
                                await transaction.CommitAsync().ConfigureAwait(false);
                                return model;
                            }
                            else
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return null;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                }
            });
            
        }

        /// <summary>
        /// 청구금액 입력
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int?> AddChargePrice(EnergyMonthChargePriceDTO? dto, string creater, int placeid)
        {

            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
            DateTime ThisDate = DateTime.Now;
            bool SaveResult = false;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        int Years = dto!.TargetDate.Year; // 해당년
                        int Month = dto!.TargetDate.Month; // 해당월

                        DateTime ThisDate = DateTime.Now; // 현재시간

                        ElecEnergyAmountTb? ElecEnergyAmountTB = await context.ElecEnergyAmountTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true &&
                                                      m.Year == Years &&
                                                      m.Month == Month &&
                                                      m.PlaceTbId == placeid)
                            .ConfigureAwait(false);

                        if (ElecEnergyAmountTB is not null)
                            return -1; // 이미 데이터가 있음.

                        ElecEnergyAmountTb NewAmountTB = new ElecEnergyAmountTb();
                        NewAmountTB.Year = Years; // 년
                        NewAmountTB.Month = Month; // 월
                        NewAmountTB.ChargePrice = dto.ChargePrice; // 청구요금
                        NewAmountTB.CreateDt = ThisDate; // 현재시간
                        NewAmountTB.CreateUser = creater;
                        NewAmountTB.UpdateDt = ThisDate; // 현재시간
                        NewAmountTB.UpdateUser = creater;
                        NewAmountTB.PlaceTbId = placeid; // 사업장

                        await context.AddAsync(NewAmountTB).ConfigureAwait(false);
                        SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if(!SaveResult)
                        {
                            await transaction.RollbackAsync();
                            return 0; // 트랜잭션 에러
                        }

                        // 월별 단가를 지정해야함.
                        List<MeterItemTb>? MeterItemList = await context.MeterItemTbs.Where(m => m.DelYn != true && m.Category == "전기" && m.PlaceTbId == placeid).ToListAsync().ConfigureAwait(false);
                        if(MeterItemList is [_, ..])
                        {
                            List<int> MeterItemIdx = MeterItemList.Select(m => m.MeterItemId).ToList();

                            List<EnergyMonthUsageTb> EnergyMonthList = await context.EnergyMonthUsageTbs
                            .Where(m => MeterItemIdx.Contains(m.MeterItemId) && 
                                        m.Year == Years && 
                                        m.Month == Month && 
                                        m.DelYn != true)
                            .ToListAsync()
                            .ConfigureAwait(false);

                            // 반복돌면서 단가입력
                            foreach(EnergyMonthUsageTb EnergyMonthUsageTB in EnergyMonthList)
                            {
                                // 해당 검침기의 단가 : 월청구금액 / 해당검침기 월 사용량
                                float MonthUnitPrice = dto.ChargePrice / (EnergyMonthUsageTB.TotalUsage ?? 0);

                                EnergyMonthUsageTB.UnitPrice = MonthUnitPrice; // 단가입력
                                EnergyMonthUsageTB.UpdateDt = ThisDate; // 현재시간
                                EnergyMonthUsageTB.UpdateUser = creater; // 수정자


                            }

                            Console.WriteLine("sadfasf");
                        }


                        await transaction.RollbackAsync(); // 테스트에서에는 커밋 XXX
                        return 1;
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
            });
        }


        /// <summary>
        /// 해당년-월 데이터 리스트 출력 
        /// - 검침기 크로스조인
        /// - 해당월의 일별데이터 전체반환
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public async Task<DayEnergyDTO?> GetMonthList(DateTime SearchDate, int placeid)
        {
            try
            {
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                // MeterItemTable의 Where 조건에 맞는 모든 List 반환
                List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                    .Where(m => m.DelYn != true &&
                                m.PlaceTbId == placeid &&
                                m.ContractTbId != null)
                    .ToListAsync();



                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                ///////////


                // 해당년도-월 의 1일과 마지막일을 구함.
                int numberOfDays = DateTime.DaysInMonth(Years, Month);
                List<DateTime> allDates = Enumerable.Range(1, numberOfDays)
                                                    .Select(day => new DateTime(Years, Month, day))
                                                    .ToList();


                List<EnergyDayUsageTb> UseDaysList = await context.EnergyDayUsageTbs
                    .Where(m => allMeterItems.Select(m => m.MeterItemId).ToList().Contains(m.MeterItemId) && 
                                m.Year == Years && 
                                m.Month == Month && 
                                m.PlaceTbId == placeid)
                    .ToListAsync()
                    .ConfigureAwait(false);

                Console.WriteLine("asdasd");

                // 해당 사업장에 속한 검침기들의 해당년도-월의 조건에 맞는 TotalPrice 합산

                /*
                float? MonthTotalPrice = await context.EnergyMonthUsageTbs
                    .Where(m => allMeterItems.Select(m => m.MeterItemId).ToList().Contains(m.MeterItemId) &&
                                m.Year == Years &&
                                m.Month == Month && 
                                m.PlaceTbId == placeid)
                    .SumAsync(m => m.TotalPrice);
                */

                ElecEnergyAmountTb? AmountTB = await context.ElecEnergyAmountTbs
                    .FirstOrDefaultAsync(m => m.DelYn != true && m.Year == Years && m.Month == Month && m.PlaceTbId == placeid);

                float MonthChargePrice = 0;
                if (AmountTB is not null)
                {
                    MonthChargePrice = AmountTB.ChargePrice!.Value;
                }

                DayEnergyDTO DTOModel = new DayEnergyDTO();
                DTOModel.TotalPrice = MonthChargePrice;


                // 그룹 만들 데이터 크로스 조인 -- 여기 왜안됨
                /*
                DTOModel.TotalList =  (from MeterTB in allMeterItems
                                      from date in allDates
                                      join EnergyDayUsageTb in UseDaysList
                                      on new { MeterTB.MeterItemId, Day = date.Day } equals new { EnergyDayUsageTb.MeterItemId, Day = EnergyDayUsageTb.Days } into usageGroup
                                      from usage in usageGroup.DefaultIfEmpty() // Left Join 처리
                                      join contractTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true) // MeterTB와 조인
                                     on MeterTB.ContractTbId equals contractTB.Id // MeterItemId를 기준으로 조인
                                      select new DayTotalEnergyDTO
                                      {
                                          MeterID = MeterTB.MeterItemId,
                                          MeterName = MeterTB.Name,
                                          ContractID = MeterTB.ContractTbId!.Value,
                                          ContractName = contractTB.Name,
                                          Date = date,
                                          DaysUseAmount = usage?.TotalAmount ?? 0 // 사용량이 없을 경우 0으로 설정
                                      })
                                    .OrderBy(x => x.MeterID) // MeterID를 기준으로 오름차순 정렬
                                    .ThenBy(x => x.Date) // Date를 기준으로 오름차순 정렬
                                    .ToList();
                */
                return DTOModel;
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
        /// 월간 - 전체 첫번째 표
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<DaysTotalContractTypeEnergyDTO>> GetContractTypeMonthList(DateTime SearchDate, int placeid)
        {
            try
            {
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                // MeterItemTable의 Where 조건에 맞는 모든 List 반환
                List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                    .Where(m => m.DelYn != true &&
                                m.PlaceTbId == placeid &&
                                m.ContractTbId != null)
                    .ToListAsync()
                    .ConfigureAwait(false);


                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                // 해당년도-월 의 1일과 마지막일자 전날 을 구함.
                int numberOfDays = DateTime.DaysInMonth(Years, Month) - 1;
                List<DateTime> allDates = Enumerable.Range(1, numberOfDays)
                                                    .Select(day => new DateTime(Years, Month, day))
                                                    .ToList();


                List<EnergyDayUsageTb> UseDaysList = await context.EnergyDayUsageTbs
                    .Where(m => allMeterItems.Select(m => m.MeterItemId).ToList().Contains(m.MeterItemId) &&
                                m.Year == Years &&
                                m.Month == Month && 
                                m.PlaceTbId == placeid)
                    .ToListAsync()
                    .ConfigureAwait(false);


                // 그룹 만들 데이터 크로스 조인 -- 여기 왜안됨
                List<DaysTotalContractTypeEnergyDTO> DTOModel = null;
                /*
                List<DaysTotalContractTypeEnergyDTO> DTOModel = (from MeterTB in allMeterItems
                              from date in allDates
                              join EnergyDayTB in UseDaysList
                              on new { MeterTB.MeterItemId, Day = date.Day } equals new { EnergyDayTB.MeterItemId, Day = EnergyDayTB.Days } into usageGroup
                              from usage in usageGroup.DefaultIfEmpty() // Left Join 처리
                              join contractTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true) // MeterTB와 조인
                              on MeterTB.ContractTbId equals contractTB.Id
                              select new
                              {
                                  ContractTypeId = MeterTB.ContractTbId!.Value,
                                  ContractTypeName = contractTB.Name,
                                  Date = date,
                                  DaysUseAmount = usage?.TotalAmount ?? 0 // 사용량이 없을 경우 0으로 설정
                              })
              .GroupBy(x => new { x.ContractTypeId, x.ContractTypeName })
              .Select(g => new DaysTotalContractTypeEnergyDTO
              {
                  ContractTypeId = g.Key.ContractTypeId,
                  ContractTypeName = g.Key.ContractTypeName,
                  TotalUse = g.Sum(x => x.DaysUseAmount), // 월합계 사용량 합산
                  DayTotalUseList = g.GroupBy(d => d.Date)
                                     .Select(dayGroup => new DayTotalUse
                                     {
                                         Date = dayGroup.Key,
                                         DaysUseAmount = dayGroup.Sum(x => x.DaysUseAmount)
                                     })
                                     .OrderBy(day => day.Date) // 날짜 기준 정렬
                                     .ToList()
              })
              .OrderBy(dto => dto.ContractTypeId) // 계약종별ID 기준 정렬
              .ToList();
                */
                return DTOModel;
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


        // 사용량비교 테스트
        public async Task<List<ContractTypeEnergyCompareUseDTO>> GetContractTypeUseCompare(DateTime SearchDate, int placeid)
        {
            try
            {
                int CurrentYears = SearchDate.Year;
                int CurrentMonth = SearchDate.Month;

                // 계약종별 리스트 구함.
                List<ContractTypeTb> ContractList = await context.ContractTypeTbs
                    .Where(m => m.DelYn != true && 
                                m.PlaceTbId == placeid)
                    .ToListAsync()
                    .ConfigureAwait(false);

                // MeterItemTable의 Where 조건에 맞는 모든 List 반환
                List<IGrouping<int?, MeterItemTb>> allMeterItems = await context.MeterItemTbs
                                                                .Where(m => ContractList.Select(c => c.Id).ToList().Contains(m.ContractTbId!.Value))
                                                                .GroupBy(m => m.ContractTbId) // Key - ContractId / Value - MeterItemTb
                                                                .ToListAsync()
                                                                .ConfigureAwait(false);

                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                // 지난달
                int previousMonth = CurrentMonth - 1;

                // 작년
                int LastYear = CurrentYears - 1;

                // 만약 현재 월이 1월이라면, 지난달은 작년 12월이다.
                if(CurrentMonth == 1)
                {
                    previousMonth = 12;
                }

                List<ContractTypeEnergyCompareUseDTO> model = new List<ContractTypeEnergyCompareUseDTO>();
                foreach(var key in allMeterItems)
                {
                    ContractTypeEnergyCompareUseDTO dto = new ContractTypeEnergyCompareUseDTO();
                    dto.ContractTypeId = key.Key!.Value; // 계약종별 ID
                    dto.ContractTypeName = ContractList.FirstOrDefault(c => c.Id == Convert.ToInt32(key.Key!.Value))!.Name;

                    List<int> meterItemIds = key.Select(m => m.MeterItemId).ToList();

                    // 현재 년-월 사용량
                    float CurrentTotal = await context.EnergyMonthUsageTbs
                                  .Where(m => meterItemIds.Contains(m.MeterItemId)
                                              && m.DelYn != true
                                              && m.Year == CurrentYears
                                              && m.Month == CurrentMonth
                                              && m.PlaceTbId == placeid)
                                  .SumAsync(m => (float?)m.TotalUsage) // nullable float로 변환
                                  .ConfigureAwait(false) ?? 0;

                    // 현재 년-지난달 사용량
                    float PreviousTotal = await context.EnergyMonthUsageTbs
                        .Where(m => meterItemIds.Contains(m.MeterItemId) &&
                        m.DelYn != true &&
                        m.Year == CurrentYears &&
                        m.Month == previousMonth &&
                        m.PlaceTbId == placeid)
                        .SumAsync(m => (float?)m.TotalUsage)
                        .ConfigureAwait(false) ?? 0;

                    // 작년 현재월 사용량
                    float LastYearTotal = await context.EnergyMonthUsageTbs
                        .Where(m => meterItemIds.Contains(m.MeterItemId) &&
                        m.DelYn != true &&
                        m.Year == LastYear &&
                        m.Month == CurrentMonth && 
                        m.PlaceTbId == placeid)
                        .SumAsync(m => (float?)m.TotalUsage)
                        .ConfigureAwait(false) ?? 0;

                    dto.ThisMonthTotalUse = CurrentTotal; // 이번달 총합
                    dto.BeforeMonthTotalUse = PreviousTotal; // 지난달 총합
                    dto.LastYearSameMonthTotalUse = LastYearTotal; // 전년동월 총합

                    model.Add(dto);
                }
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

        public async Task<List<ContractTypeEnergyComparePriceDTO>> GetContractTypePriceCompare(DateTime SearchDate, int placeid)
        {
            try
            {
                int CurrentYears = SearchDate.Year;
                int CurrentMonth = SearchDate.Month;

                // 계약종별 리스트 구함.
                List<ContractTypeTb> ContractList = await context.ContractTypeTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                    .ToListAsync().ConfigureAwait(false);

                // MeterItemTable의 Where 조건에 맞는 모든 List반환
                List<IGrouping<int?, MeterItemTb>> allMeterItems = await context.MeterItemTbs
                                    .Where(m => ContractList.Select(c => c.Id).ToList().Contains(m.ContractTbId!.Value))
                                    .GroupBy(m => m.ContractTbId)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                // 지난달
                int previousMonth = CurrentMonth - 1;

                // 작년
                int LastYear = CurrentYears - 1;

                // 만약 현재 월이 1월이라면, 지난달은 작년 12월이다.
                if(CurrentMonth == 1)
                {
                    previousMonth = 12;
                }

                List<ContractTypeEnergyComparePriceDTO> model = new List<ContractTypeEnergyComparePriceDTO>();

                foreach(var key in allMeterItems)
                {
                    ContractTypeEnergyComparePriceDTO dto = new ContractTypeEnergyComparePriceDTO();
                    dto.ContractTypeId = key.Key!.Value; // 계약종별 ID
                    dto.ContractTypeName = ContractList.FirstOrDefault(c => c.Id == Convert.ToInt32(key.Key!.Value))!.Name;

                    List<int> meterItemIds = key.Select(m => m.MeterItemId).ToList();

                    // 현재 년-월 단가
                    float CurrentTotal = await context.EnergyMonthUsageTbs
                            .Where(m => meterItemIds.Contains(m.MeterItemId)
                                        && m.DelYn != true
                                        && m.Year == CurrentYears
                                        && m.Month == CurrentMonth
                                        && m.PlaceTbId == placeid)
                            .SumAsync(m => (float?)m.TotalUsage) // nullable float로 변환
                            .ConfigureAwait(false) ?? 0;

                    //float CurrentCharge = await 

                }

                return null;

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

        public async Task<List<DayTotalMeterEnergyDTO>> GetMeterMonthList(DateTime SearchDate, List<int> MeterId, int placeid)
        {
            try
            {
                int Years = SearchDate.Year;
                int Month = SearchDate.Month;

                List<MeterItemTb>? allMeterItems = await context.MeterItemTbs
                    .Where(m => m.DelYn != true &&
                                m.PlaceTbId == placeid)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (allMeterItems is null || !allMeterItems.Any())
                    return null;

                // 해당년도-월의 1일과 마지막일을 구함.
                int numberOfDays = DateTime.DaysInMonth(Years, Month);
                List<DateTime> allDates = Enumerable.Range(1, numberOfDays)
                    .Select(day => new DateTime(Years, Month, day))
                    .ToList();

                List<EnergyDayUsageTb> UseDaysList = await context.EnergyDayUsageTbs
                  .Where(m => allMeterItems.Select(m => m.MeterItemId).ToList().Contains(m.MeterItemId) &&
                              m.Year == Years &&
                              m.Month == Month && 
                              m.PlaceTbId == placeid)
                  .ToListAsync()
                  .ConfigureAwait(false);
                List<DayTotalMeterEnergyDTO> result = null;

                // 여기왜안됨
                /*
                List<DayTotalMeterEnergyDTO> result = (from MeterTB in allMeterItems
                              from date in allDates
                              join usage in UseDaysList
                              on new { MeterTB.MeterItemId, Day = date.Day } equals new { usage.MeterItemId, Day = usage.Days } into usageGroup
                              from usageData in usageGroup.DefaultIfEmpty() // Left Join 처리
                              join contractTB in context.ContractTypeTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                              on MeterTB.ContractTbId equals contractTB.Id
                              select new
                              {
                                  ContractTypeId = MeterTB.ContractTbId!.Value,
                                  ContractTypeName = contractTB.Name,
                                  MeterId = MeterTB.MeterItemId,
                                  MeterName = MeterTB.Name,
                                  Date = date,
                                  DaysUseAmount = usageData?.TotalAmount ?? 0 // 해당 날짜에 데이터가 없으면 0으로 설정
                              })
              .GroupBy(x => new { x.ContractTypeId, x.ContractTypeName, x.MeterId, x.MeterName })
              .Select(g => new DayTotalMeterEnergyDTO
              {
                  ContractTypeId = g.Key.ContractTypeId,
                  ContractTypeName = g.Key.ContractTypeName,
                  MeterId = g.Key.MeterId,
                  MeterName = g.Key.MeterName,
                  TotalUse = g.Sum(x => x.DaysUseAmount), // 월합계 사용량 합산
                  DayTotalUseList = g.Select(d => new DayTotalUse
                  {
                      Date = d.Date,
                      DaysUseAmount = d.DaysUseAmount
                  })
                  .OrderBy(day => day.Date) // 날짜 기준 정렬
                  .ToList()
              })
              .OrderBy(dto => dto.MeterId) // MeterId 기준 정렬
              .ThenBy(dto => dto.DayTotalUseList.First().Date) // 첫 번째 날짜 기준 정렬
              .ToList();
                */
               return result;
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

      
    }
}
