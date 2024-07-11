using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;

namespace FamTec.Server.Services.Building.Value
{
    public interface IBuildingValueService
    {
        // value - 업데이트 (단일)
        public ValueTask<ResponseUnit<UpdateValueDTO?>> UpdateValueService(HttpContext? context, UpdateValueDTO? dto);


        // Value - 삭제 (단일)
        public ValueTask<ResponseUnit<bool?>> DeleteValueService(HttpContext? context, int? valueid);
    }
}
