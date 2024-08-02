using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Room;

namespace FamTec.Server.Services.Room
{
    public interface IRoomService
    {
        /// <summary>
        /// 선택된 층에 공간 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<RoomDTO>?> AddRoomService(HttpContext? context, RoomDTO? dto);

        /// <summary>
        /// 로그인한 사업장의 모든 공간 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<RoomListDTO>> GetRoomListService(HttpContext? context);

        /// <summary>
        /// 공간 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateRoomService(HttpContext? context, UpdateRoomDTO? dto);

        /// <summary>
        /// 공간 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteRoomService(HttpContext? context, List<int>? del);

    }
}
