using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Value
{
    public interface IFacilityValueService
    {
        public Task<ResponseUnit<AddValueDTO>> AddValueService(HttpContext context, AddValueDTO dto);

        // value - 업데이트 (단일)
        public Task<ResponseUnit<UpdateValueDTO>> UpdateValueService(HttpContext context, UpdateValueDTO dto);

        // value - 삭제 (단일)
        public Task<ResponseUnit<bool?>> DeleteValueService(HttpContext context, int valueid);

    }
}
