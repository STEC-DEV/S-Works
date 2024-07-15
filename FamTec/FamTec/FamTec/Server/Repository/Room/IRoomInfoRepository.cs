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

        ValueTask<bool?> UpdateRoomInfo(RoomTb? model);
        ValueTask<bool?> DeleteRoomInfo(RoomTb? model);

    }
}
