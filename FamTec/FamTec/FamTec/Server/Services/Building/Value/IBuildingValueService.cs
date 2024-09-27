using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;

namespace FamTec.Server.Services.Building.Value
{
    public interface IBuildingValueService
    {

        /// <summary>
        /// 값 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddValueDTO>> AddValueService(HttpContext context, AddValueDTO dto);

        /// <summary>
        /// value - 업데이트 (단일)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UpdateValueDTO>> UpdateValueService(HttpContext context, UpdateValueDTO dto);


        /// <summary>
        /// Value - 삭제 (단일)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteValueService(HttpContext context, int valueid);
    }
}
