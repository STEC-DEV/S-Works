using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem.ItemKey
{
    public interface IBuildingItemKeyInfoRepository
    {
        /// <summary>
        /// 그룹의 KEY 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<BuildingItemkeyTb?> AddAsync(BuildingItemkeyTb? model);

        /// <summary>
        /// 그룹 KEY 리스트 상세검색 groupitemid로 검색
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemkeyTb>?> GetAllKeyList(int? groupitemid);

        /// <summary>
        /// 그룹 KEY 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        ValueTask<BuildingItemkeyTb?> GetKeyInfo(int? keyid);
        
        /// <summary>
        /// 그룹 KEY 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateKeyInfo(BuildingItemkeyTb? model);

        /// <summary>
        /// 그룹 KEY 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteKeyInfo(BuildingItemkeyTb? model);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemkeyTb>?> ContainsKeyList(List<int> GroupItemId);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemkeyTb>?> NotContainsKeyList(List<int> GroupItemId);
    }
}
