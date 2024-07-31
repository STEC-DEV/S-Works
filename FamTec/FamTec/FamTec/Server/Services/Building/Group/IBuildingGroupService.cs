using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;

namespace FamTec.Server.Services.Building.Group
{
    public interface IBuildingGroupService
    {
        public ValueTask<ResponseUnit<AddGroupDTO?>> AddBuildingGroupService(HttpContext? context, AddGroupDTO? dto);

        public ValueTask<ResponseList<GroupListDTO?>> GetBuildingGroupListService(HttpContext? context, int? buildingId);

        public ValueTask<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext? context, UpdateGroupDTO? dto);

        public ValueTask<ResponseUnit<bool?>> DeleteGroupService(HttpContext? context, int? groupid);

        public ValueTask<ResponseUnit<AddGroupInfoDTO?>> AddBuildingGroupInfoService(HttpContext? context, AddGroupInfoDTO? dto);
    }
}
