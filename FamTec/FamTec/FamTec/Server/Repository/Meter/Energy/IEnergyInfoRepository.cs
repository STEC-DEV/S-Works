using FamTec.Shared.Model;

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
        /// 오늘 입력했는지 검색
        ///     - 입력했으면 수정으로 해야함.
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public ValueTask<EnergyUsageTb?> GetDayEnergy(DateTime SearchDate);

    }
}
