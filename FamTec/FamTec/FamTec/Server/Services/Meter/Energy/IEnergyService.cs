using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Energy;

namespace FamTec.Server.Services.Meter.Energy
{
    public interface IEnergyService
    {
        /// <summary>
        /// 에너지 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddEnergyDTO>> AddEnergyService(HttpContext context, AddEnergyDTO dto);

        /// <summary>
        /// 선택된 년도의 달의 일별 합산 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        Task<ResponseList<DayEnergyDTO>> GetMonthListService(HttpContext context, DateTime SearchDate);

        /// <summary>
        /// 선택된 년도의 선택된 검침기의 달의 일별 합산 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SearchDate"></param>
        /// <param name="MeterId"></param>
        /// <returns></returns>
        Task<ResponseList<DayEnergyDTO>> GetMonthSelectListService(HttpContext context, DateTime SearchDate, List<int> MeterId);

        /// <summary>
        /// 선택된 년도의 월별 통계
        /// </summary>
        /// <param name="context"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<ResponseList<YearsTotalEnergyDTO>> GetYearListService(HttpContext context, int year);

        /// <summary>
        /// 선택된 년도의 선택된 검침기의 월별 통계
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MeterId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<ResponseList<YearsTotalEnergyDTO>> GetYearSelectListService(HttpContext context, List<int> MeterId, int year);

        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 전체출력
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        Task<ResponseList<DayEnergyDTO>> GetDaysListService(HttpContext context, DateTime StartDate, DateTime EndDate);

        /// <summary>
        /// 선택된 검침기의 선택된 일자 사이의 데이터 리스트 출력 - 선택
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MeterId"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        Task<ResponseList<DayEnergyDTO>> GetDaysSelectListService(HttpContext context, List<int> MeterId, DateTime StartDate, DateTime EndDate);

    }
}
