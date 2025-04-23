using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;

namespace FamTec.Server.Services.Building.Group
{
    public interface IBuildingGroupService
    {
        /// <summary>
        /// 그룹 - 키 - 값 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> AddBuildingGroupService(List<AddGroupDTO> dto);

        /// <summary>
        /// 그룹만 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddGroupInfoDTO>> AddBuildingGroupInfoService(AddGroupInfoDTO dto);

        /// <summary>
        /// detail -- buildingid --> GroupList 랑 ItemList 전체다 한번에 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public Task<ResponseList<GroupListDTO?>> GetBuildingGroupListService(int buildingId);

        /// <summary>
        /// Update group 명칭만 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateGroupNameService(UpdateGroupDTO dto);

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteGroupService(int groupid);

        
    }
}
