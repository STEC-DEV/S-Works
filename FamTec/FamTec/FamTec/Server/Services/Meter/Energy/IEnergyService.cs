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
        public ValueTask<ResponseUnit<AddEnergyDTO>> AddEnergyService(HttpContext context, AddEnergyDTO dto);

        /// <summary>
        /// 선택된 년도의 달의 일별 합산 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        ValueTask<ResponseList<DaysTotalEnergyDTO>> GetMonthListService(HttpContext context, DateTime SearchDate);
    }
}
