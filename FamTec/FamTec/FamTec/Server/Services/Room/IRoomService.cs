using FamTec.Server.Helpers;
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
        public Task<ResponseModel<RoomDTO>> AddRoomService(RoomDTO dto);

        /// <summary>
        /// 로그인한 사업장의 모든 공간 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<RoomListDTO>>> GetRoomListService();

        /// <summary>
        /// 공간 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateRoomService(UpdateRoomDTO dto);

        /// <summary>
        /// 공간 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteRoomService(List<int> del);

        /// <summary>
        /// 사업장에 해당하는 전체건물 - 전체층 - 전체공간 Group
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<PlaceRoomListDTO>>> GetPlaceAllGroupRoomInfo();

        /// <summary>
        /// 공간ID로 공간명칭 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        Task<ResponseModel<string?>> GetRoomNameService(int roomid);
    }
}
