using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem.Group
{
    public interface IBuildingGroupItemInfoRepository
    {
        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BuildingItemGroupTb?> AddAsync(BuildingItemGroupTb model);

        /// <summary>
        /// 그룹리스트 상세검색 buildingid로 검색
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        Task<List<BuildingItemGroupTb>?> GetAllGroupList(int buildingid);

        /// <summary>
        /// 그룹 상세검색 groupid로 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        Task<BuildingItemGroupTb?> GetGroupInfo(int groupid);


        /// <summary>
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateGroupInfo(BuildingItemGroupTb model);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteGroupInfo(BuildingItemGroupTb model);

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> DeleteGroupInfo(int groupid, string deleter);

        /// <summary>
        /// 넘어온 Id에 포함되어있는 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemGroupTb>?> ContainsGroupList(List<int> GroupId, int buildingid);

        /// <summary>
        /// 넘어온 Id에 포함되어있지 않은 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        Task<List<BuildingItemGroupTb>?> NotContainsGroupList(List<int> GroupId, int buildingid);
    }
}
