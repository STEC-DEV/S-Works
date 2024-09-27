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
        /// 해당년-월 데이터 전체 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public Task<List<DayEnergyDTO>?> GetMonthList(DateTime SearchDate, int placeid);

        /// <summary>
        /// 해당년-월 데이터 선택된 검침기 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<DayEnergyDTO>?> GetMeterMonthList(DateTime SearchDate, List<int> MeterId, int placeid);

        /// <summary>
        /// 년도별 통계
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<YearsTotalEnergyDTO>> GetYearsList(int year, int placeid);

        /// <summary>
        /// 년도별 선택된 검침기에 대한 통계
        /// </summary>
        /// <param name="year"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<YearsTotalEnergyDTO>> GetMeterYearsList(int year, List<int> MeterId, int placeid);

        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 출력 - 전체
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<DayEnergyDTO>> GetDaysList(DateTime StartDate, DateTime EndDate, int placeid);

        /// <summary>
        /// 선택된 검침기의 선택된 일자 사이의 데이터 리스트 출력 - 선택
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="MeterId"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<DayEnergyDTO>> GetMeterDaysList(DateTime StartDate, DateTime EndDate, List<int> MeterId, int placeid);

        /// <summary>
        /// 사용량비교
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<GetUseCompareDTO>> GetUseCompareList(DateTime SearchDate, int placeid);
    }
}
