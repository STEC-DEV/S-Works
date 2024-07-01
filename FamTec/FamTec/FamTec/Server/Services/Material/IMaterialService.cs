using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public interface IMaterialService
    {
        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddMaterialDTO>?> AddMaterialService(HttpContext? context, AddMaterialDTO? dto);

        /// <summary>
        /// 사업장의 전체 자재리스트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaterialListDTO>?> GetPlaceMaterialListService(HttpContext? context);

    }
}
