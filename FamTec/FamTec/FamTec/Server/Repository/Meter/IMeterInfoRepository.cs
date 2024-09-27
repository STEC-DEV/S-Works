using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Meter
{
    public interface IMeterInfoRepository
    {
        /// <summary>
        /// 검침기 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<MeterItemTb?> AddAsync(MeterItemTb model);

        /// <summary>
        /// 사업장에 속한 검침기 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<List<MeterItemTb>?> GetAllMeterList(int placeid);

        /// <summary>
        /// 사업장에 속한 해당 카테고리 검침기 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public Task<List<MeterItemTb>?> GetAllCategoryMeterList(int placeid, string category);

        /// <summary>
        /// 검침기 명칭으로 검색
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<MeterItemTb?> GetMeterName(int placeid, string name);

    }
}
