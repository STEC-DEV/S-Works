using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Building.Group.Key;

namespace FamTec.Server.Repository.Building.SubItem.ItemKey
{
    public interface IBuildingItemKeyInfoRepository
    {
        /// <summary>
        /// 그룹의 KEY 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BuildingItemKeyTb?> AddAsync(BuildingItemKeyTb model);

        /// <summary>
        /// 그룹 KEY 리스트 상세검색 groupitemid로 검색
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        Task<List<BuildingItemKeyTb>?> GetAllKeyList(int groupitemid);

        /// <summary>
        /// 그룹 KEY 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        Task<BuildingItemKeyTb?> GetKeyInfo(int keyid);
        
        /// <summary>
        /// 그룹 KEY 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateKeyInfo(BuildingItemKeyTb model);

        /// <summary>
        /// 그룹 KEY - VALUE 업데이트
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        Task<bool?> UpdateKeyInfo(UpdateKeyDTO dto, string updater);

        /// <summary>
        /// 그룹 KEY 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteKeyInfo(BuildingItemKeyTb model);

        /// <summary>
        /// 그룹 KEY 리스트 삭제 - Value 까지 삭제됨
        /// </summary>
        /// <param name="KeyList"></param>
        /// <returns></returns>
        Task<bool?> DeleteKeyList(List<int> KeyList, string deleter);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemKeyTb>?> ContainsKeyList(List<int> GroupItemId);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemKeyTb>?> NotContainsKeyList(List<int> GroupItemId);
    }
}
