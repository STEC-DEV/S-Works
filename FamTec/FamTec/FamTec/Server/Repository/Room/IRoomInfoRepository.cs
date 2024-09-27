using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Room;

namespace FamTec.Server.Repository.Room
{
    public interface IRoomInfoRepository
    {
        /// <summary>
        /// 공간 삭제가능여부 체크
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        Task<bool?> DelRoomCheck(int roomid);

        /// <summary>
        /// 공간 정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<RoomTb?> AddAsync(RoomTb model);

        /// <summary>
        /// 층에 해당하는 공간 List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<RoomTb>?> GetRoomList(int flooridx);

        /// <summary>
        /// 층에 해당하는 공간 List 조회
        /// </summary>
        /// <param name="FloorList"></param>
        /// <returns></returns>
        Task<List<RoomTb>?> GetFloorRoomList(List<FloorTb> FloorList);

        /// <summary>
        /// 공간 인덱스로 공간 검색
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        Task<RoomTb?> GetRoomInfo(int roomidx);

        /// <summary>
        /// 공간 삭제검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        Task<bool?> RoomDeleteCheck(int placeid, int roomidx);

        /// <summary>
        /// 공간정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateRoomInfo(RoomTb model);

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteRoomInfo(RoomTb model);

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> DeleteRoomInfo(List<int> idx, string deleter);

        /// <summary>
        /// 사업장에 해당하는 전체건물 - 전체층 - 전체공간 Group
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<PlaceRoomListDTO>?> GetPlaceAllGroupRoomInfo(int placeid);
    }
}
