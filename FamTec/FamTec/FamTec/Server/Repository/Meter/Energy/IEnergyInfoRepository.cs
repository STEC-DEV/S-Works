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
        public ValueTask<EnergyUsageTb?> AddAsync(EnergyUsageTb model);

        /// <summary>
        /// 해당년-월 데이터 리스트 출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public ValueTask<List<DayEnergyDTO>?> GetMonthList(DateTime SearchDate, int placeid);

    }
}
