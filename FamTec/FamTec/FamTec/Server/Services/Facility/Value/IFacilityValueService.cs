using FamTec.Server.Helpers;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Value
{
    public interface IFacilityValueService
    {
        public Task<ResponseModel<AddValueDTO>> AddValueService(AddValueDTO dto);

        // value - 업데이트 (단일)
        public Task<ResponseModel<UpdateValueDTO>> UpdateValueService(UpdateValueDTO dto);

        // value - 삭제 (단일)
        public Task<ResponseModel<bool?>> DeleteValueService(int valueid);

    }
}
