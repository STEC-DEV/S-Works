using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;

namespace FamTec.Server.Services.Building.Key
{
    public interface IBuildingKeyService
    {
        /// <summary>
        /// 키 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto);

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(HttpContext context, UpdateKeyDTO dto);

        /// <summary>
        /// 키 - value 삭제 단일 묶음 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteKeyService(HttpContext context, int KeyId);

    }
}
