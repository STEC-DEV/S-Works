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
        ValueTask<BuildingItemKeyTb?> AddAsync(BuildingItemKeyTb? model);

        /// <summary>
        /// 그룹 KEY 리스트 상세검색 groupitemid로 검색
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemKeyTb>?> GetAllKeyList(int? groupitemid);

        /// <summary>
        /// 그룹 KEY 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        ValueTask<BuildingItemKeyTb?> GetKeyInfo(int? keyid);
        
        /// <summary>
        /// 그룹 KEY 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateKeyInfo(BuildingItemKeyTb? model);

        /// <summary>
        /// 그룹 KEY - VALUE 업데이트
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateKeyInfo(UpdateKeyDTO? dto, string? updater);

        /// <summary>
        /// 그룹 KEY 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteKeyInfo(BuildingItemKeyTb? model);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemKeyTb>?> ContainsKeyList(List<int> GroupItemId);

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<BuildingItemKeyTb>?> NotContainsKeyList(List<int> GroupItemId);
    }
}
