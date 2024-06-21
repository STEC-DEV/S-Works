using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Floor;

namespace FamTec.Server.Services.Floor
{
    public interface IFloorService
    {
        /// <summary>
        /// 건물에 속해있는 층 리스트 반환
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<FloorDTO>?> GetFloorListService(int? buildingtbid);

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseModel<string>?> DeleteFloorService(List<int>? index, SessionInfo? session);
    }
}
