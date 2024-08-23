using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Key
{
    public interface IFacilityKeyService
    {
        /// <summary>
        /// 키 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto);

        /// <summary>
        /// 키 - value 업데이트 (키-value) 단일 묶음 업데이트
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

        /// <summary>
        /// 키 List - Value 삭제 리스트 묶음 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeletKeyListService(HttpContext? context, List<int> KeyId);

    }
}
