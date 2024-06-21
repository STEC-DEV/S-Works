using FamTec.Shared;
using FamTec.Shared.DTO;
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
        public ValueTask<ResponseUnit<bool>> AddBuildingService(HttpContext context, BuildingsDTO? dto);

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물리스트 조회
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext? context);

        /// <summary>
        /// 건물 정보 삭제
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string>?> DeleteBuildingService(List<int>? index, SessionInfo? session);

    }
}
