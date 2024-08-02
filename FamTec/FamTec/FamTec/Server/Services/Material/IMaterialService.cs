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
        public ValueTask<ResponseUnit<AddMaterialDTO>?> AddMaterialService(HttpContext? context, AddMaterialDTO? dto, IFormFile? files);

        /// <summary>
        /// 사업장의 전체 자재리스트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaterialListDTO>?> GetPlaceMaterialListService(HttpContext? context);

        /// <summary>
        /// 자재 상세정보 보기
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<DetailMaterialDTO>?> GetDetailMaterialService(HttpContext? context,int? materialid);

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateMaterialService(HttpContext? context, UpdateMaterialDTO? dto, IFormFile? files);

        /// <summary>
        /// 자재정보 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteMaterialService(HttpContext? context, List<int>? delIdx);
    }
}
