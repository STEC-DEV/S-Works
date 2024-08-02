using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Room;

namespace FamTec.Server.Repository.Room
{
    public interface IRoomInfoRepository
    {
        /// <summary>
        /// 공간 정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<RoomTb?> AddAsync(RoomTb? model);

        /// <summary>
        /// 층에 해당하는 공간 List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<List<RoomTb>?> GetRoomList(int? flooridx);

        /// <summary>
        /// 공간 인덱스로 공간 검색
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        ValueTask<RoomTb?> GetRoomInfo(int? roomidx);

        /// <summary>
        /// 공간 삭제검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        ValueTask<bool?> RoomDeleteCheck(int? placeid, int? roomidx);

        /// <summary>
        /// 공간정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateRoomInfo(RoomTb? model);

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteRoomInfo(RoomTb? model);

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteRoomInfo(List<int>? idx, string? deleter);
    }
}
