using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Machine.Value
{
    public interface IMachineValueService
    {
        public ValueTask<ResponseUnit<AddValueDTO?>> AddValueService(HttpContext? context, AddValueDTO? dto);

        // value - 업데이트 (단일)
        public ValueTask<ResponseUnit<UpdateValueDTO?>> UpdateValueService(HttpContext? conteeext, UpdateValueDTO? dto);

        // value - 삭제 (단일)
        public ValueTask<ResponseUnit<bool?>> DeleteValueService(HttpContext? context, int? valueid);
            
    }
}
