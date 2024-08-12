using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Group
{
    public interface IFacilityGroupService
    {
        /// <summary>
        /// 그룹 - 키 - 값 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddGroupDTO>> AddFacilityGroupService(HttpContext context, AddGroupDTO dto);

        /// <summary>
        /// 그룹만 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddGroupInfoDTO>> AddFacilityGroupInfoService(HttpContext context, AddGroupInfoDTO dto);
        
        /// <summary>
        /// detail -- groupid --> GroupList랑 ItemList 전체다 한번에 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<GroupListDTO>> GetFacilityGroupListService(HttpContext context, int facilityid);

        /// <summary>
        /// Update Group 명칭만 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext context, UpdateGroupDTO dto);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteGroupService(HttpContext context, int groupid);

    }
}
