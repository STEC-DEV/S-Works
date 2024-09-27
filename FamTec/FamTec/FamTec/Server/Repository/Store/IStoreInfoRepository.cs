using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Store
{
    public interface IStoreInfoRepository
    {

        Task<StoreTb?> AddAsync(StoreTb model);

        /// <summary>
        /// 사업장의 입출고 총 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<int> GetPlaceInOutCount(int placeid);
        
        /// <summary>
        /// 사업장별 INOUT 리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<InOutHistoryListDTO>?> GetInOutList(int placeid);

        /// <summary>
        /// 사업장별 INOUT 페이지네이션 리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<List<InOutHistoryListDTO>?> GetInOutPageNationList(int placeid, int pagenumber, int pagesize);
    }
}
