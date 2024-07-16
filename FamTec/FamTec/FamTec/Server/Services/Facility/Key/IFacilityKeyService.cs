using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Key
{
    public interface IFacilityKeyService
    {
        // 키 추가
        public ValueTask<ResponseUnit<AddKeyDTO?>> AddKeyService(HttpContext? context, AddKeyDTO? dto);

        // 키 - value 업데이트 (키-value) 단일 묶음 업데이트
        public ValueTask<ResponseUnit<UpdateKeyDTO?>> UpdateKeyService(HttpContext? context, UpdateKeyDTO? dto);

        // 키 - value 삭제 단일 묶음 삭제
        public ValueTask<ResponseUnit<bool?>> DeleteKeyService(HttpContext? context, int? KeyId);

    }
}
