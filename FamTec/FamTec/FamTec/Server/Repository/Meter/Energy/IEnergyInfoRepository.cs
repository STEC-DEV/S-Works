using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Meter.Energy;

namespace FamTec.Server.Repository.Meter.Energy
{
    public interface IEnergyInfoRepository
    {
        /// <summary>
        /// 일일 검침값 입력
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<EnergyDayUsageTb?> AddAsync(EnergyDayUsageTb model);

        /// <summary>
        /// 해당 년-월-일 값 뽑아오기
        ///     - 어떻게 쓰일진 모르겠지만 아직까진 해당날짜에 데이터가 있는지 여부
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public Task<EnergyDayUsageTb?> GetUsageDaysInfo(int meterid, int year, int month, int day, int placeid);

        /// <summary>
        /// 해당년-월 데이터 전체 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public Task<DayEnergyDTO?> GetMonthList(DateTime SearchDate, int placeid);

        /// <summary>
        /// 청구금액 입력
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<int?> AddChargePrice(EnergyMonthChargePriceDTO? dto, string creater, int placeid);

        public Task<List<DaysTotalContractTypeEnergyDTO>> GetContractTypeMonthList(DateTime SearchDate, int placeid);

        public Task<List<DayTotalMeterEnergyDTO>> GetMeterMonthList(DateTime SearchDate, List<int> MeterId, int placeid);



        public Task<List<ContractTypeEnergyCompareUseDTO>> GetContractTypeUseCompare(DateTime SearchDate, int placeid);


        public Task<List<ContractTypeEnergyComparePriceDTO>> GetContractTypePriceCompare(DateTime SearchDate, int placeid);








    }
}
