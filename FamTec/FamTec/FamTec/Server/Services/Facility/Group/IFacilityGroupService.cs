using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Group
{
    public interface IFacilityGroupService
    {
        public ValueTask<ResponseUnit<AddGroupDTO?>> AddFacilityGroupService(HttpContext? context, AddGroupDTO? dto);

        public ValueTask<ResponseList<GroupListDTO?>> GetFacilityGroupListService(HttpContext? context, int? facilityid);

        public ValueTask<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext? context, UpdateGroupDTO? dto);

        public ValueTask<ResponseUnit<bool?>> DeleteGroupService(HttpContext? context, int? groupid);

    }
}
