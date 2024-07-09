using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem.Group
{
    public interface IGroupItemInfoRepository
    {
        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<GroupitemTb?> AddAsync(GroupitemTb? model);

        /// <summary>
        /// 그룹리스트 상세검색 buildingid로 검색
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        ValueTask<List<GroupitemTb>?> GetAllGroupList(int? buildingid);

        /// <summary>
        /// 그룹 상세검색 groupid로 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        ValueTask<GroupitemTb?> GetGroupInfo(int? groupid);


        /// <summary>
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateGroupInfo(GroupitemTb? model);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteGroupInfo(GroupitemTb? model);

        /// <summary>
        /// 넘어온 Id에 포함되어있는 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<GroupitemTb>?> ContainsGroupList(List<int>? GroupId, int buildingid);

        /// <summary>
        /// 넘어온 Id에 포함되어있지 않은 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<GroupitemTb>?> NotContainsGroupList(List<int>? GroupId, int buildingid);
    }
}
