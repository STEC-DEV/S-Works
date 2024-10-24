﻿using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Repository.Facility.Group
{
    public interface IFacilityGroupItemInfoRepository
    {
        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<FacilityItemGroupTb?> AddAsync(FacilityItemGroupTb model);

        /// <summary>
        /// 설비 그룹 한번에 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<int> AddGroupAsync(List<AddGroupDTO> dto, string creater);

        /// <summary>
        /// 설비ID에 포함되어있는 그룹List 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        Task<List<FacilityItemGroupTb>?> GetAllGroupList(int facilityId);

        /// <summary>
        /// 그룹ID로 모델 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        Task<FacilityItemGroupTb?> GetGroupInfo(int groupid);

        /// <summary>
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateGroupInfo(FacilityItemGroupTb model);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteGroupInfo(FacilityItemGroupTb model);

        /// <summary>
        /// 그룹 삭제 -2
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> DeleteGroupInfo(int groupid, string deleter);

    }
}
