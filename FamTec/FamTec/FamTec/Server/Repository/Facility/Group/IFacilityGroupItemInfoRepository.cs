using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Facility.Group
{
    public interface IFacilityGroupItemInfoRepository
    {
        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<FacilityItemGroupTb?> AddAsync(FacilityItemGroupTb model);

        /// <summary>
        /// 설비ID에 포함되어있는 그룹List 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        ValueTask<List<FacilityItemGroupTb>?> GetAllGroupList(int facilityId);

        /// <summary>
        /// 그룹ID로 모델 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        ValueTask<FacilityItemGroupTb?> GetGroupInfo(int groupid);

        /// <summary>
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateGroupInfo(FacilityItemGroupTb model);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteGroupInfo(FacilityItemGroupTb model);

    }
}
