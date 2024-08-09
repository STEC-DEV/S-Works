using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Store
{
    public interface IStoreInfoRepository
    {
        ValueTask<StoreTb?> AddAsync(StoreTb model);

        /// <summary>
        /// 사업장별 INOUT 리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<InOutHistoryListDTO>?> GetInOutList(int placeid);

    }
}
