using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key;

namespace FamTec.Server.Services.Building.Key
{
    public interface IBuildingKeyService
    {
        // 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        public ValueTask<ResponseUnit<UpdateKeyDTO?>> UpdateKeyService(HttpContext? context, UpdateKeyDTO? dto);

        // 키 - value 삭제 단일 묶음 삭제
        public ValueTask<ResponseUnit<bool?>> DeleteKeyService(HttpContext? context, int? KeyId);

    }
}
