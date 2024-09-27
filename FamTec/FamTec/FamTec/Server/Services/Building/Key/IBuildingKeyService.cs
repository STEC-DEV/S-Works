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
        public Task<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto);

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(HttpContext context, UpdateKeyDTO dto);

        /// <summary>
        /// 키 - value 삭제 단일 묶음 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteKeyService(HttpContext context, int KeyId);

        /// <summary>
        /// 키 List - Value 삭제 리스트 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteKeyListService(HttpContext context, List<int> KeyId);
    }
}
