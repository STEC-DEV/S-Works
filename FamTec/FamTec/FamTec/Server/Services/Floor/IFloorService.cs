using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Floor;

namespace FamTec.Server.Services.Floor
{
    public interface IFloorService
    {
        /// <summary>
        /// 층 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<FloorDTO>?> AddFloorService(HttpContext? context, FloorDTO? dto);

        /// <summary>
        /// 건물에 속해있는 층 리스트 반환
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<FloorDTO>?> GetFloorListService(int? buildingtbid);

        /// <summary>
        /// 층 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateFloorService(HttpContext? context, UpdateFloorDTO? dto);

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> DeleteFloorService(HttpContext? context, List<int>? del);

    }
}
