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
        public Task<ResponseUnit<AddKeyDTO>> AddKeyService(AddKeyDTO dto);

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(UpdateKeyDTO dto);

        /// <summary>
        /// 키 - value 삭제 단일 묶음 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteKeyService(int KeyId);

        /// <summary>
        /// 키 List - Value 삭제 리스트 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteKeyListService(List<int> KeyId);
    }
}
