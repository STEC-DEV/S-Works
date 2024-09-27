using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem.ItemValue
{
    public interface IBuildingItemValueInfoRepository
    {
        /// <summary>
        /// 아이템 KEY에 대한 Value추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BuildingItemValueTb?> AddAsync(BuildingItemValueTb model);

        /// <summary>
        /// 아이템 Value 리스트 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        Task<List<BuildingItemValueTb>?> GetAllValueList(int keyid);

        /// <summary>
        /// 아이템 Value 상세검색 valueid로 검색
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        Task<BuildingItemValueTb?> GetValueInfo(int valueid);

        /// <summary>
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateValueInfo(BuildingItemValueTb model);

        /// <summary>
        /// 아이템 Value 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteValueInfo(BuildingItemValueTb model);

        /// <summary>
        /// 넘어온 keyItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemValueTb>?> ContainsKeyList(List<int> KeyitemId);

        /// <summary>
        /// 넘어온 keyItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemValueTb>?> NotContainsKeyList(List<int> KeyitemId);

    }
}
