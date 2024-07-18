using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Store
{
    public interface IStoreInfoRepository
    {
        ValueTask<StoreTb?> AddAsync(StoreTb? model);

        /// <summary>
        /// 품목의 사용자재 List 출력
        /// </summary>
        /// <param name="Materialid"></param>
        /// <returns></returns>
        ValueTask<List<StoreListDTO>> UsedMaterialList(int? Materialid);

        ValueTask<List<StoreTb>?> GetStoreList(int? placeid); // DTO 반환해야함.




    }
}
