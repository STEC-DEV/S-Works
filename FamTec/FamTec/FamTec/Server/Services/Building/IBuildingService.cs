using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;

namespace FamTec.Server.Services.Building
{
    public interface IBuildingService
    {
        /// <summary>
        /// 건물추가
        /// </summary>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool>> AddBuildingService(HttpContext context, AddBuildingDTO? dto, IFormFile? files);

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물리스트 조회
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext? context);

        

        /// <summary>
        /// 건물 상세 정보 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<DetailBuildingDTO>?> GetDetailBuildingService(HttpContext? context, int? buildingId);

        /// <summary>
        /// 건물 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateBuildingService(HttpContext? context, DetailBuildingDTO? dto, IFormFile? files);

        /// <summary>
        /// 건물 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> DeleteBuildingService(HttpContext? context, List<int>? buildingid);
    }
}
