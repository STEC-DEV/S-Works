using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Room
{
    public class RoomInfoRepository : IRoomInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public RoomInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 공간정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<RoomTb?> AddAsync(RoomTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.RoomTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



        /// <summary>
        /// 층에 해당하는 공간 List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<List<RoomTb>?> GetRoomList(int? flooridx)
        {
            try
            {
                if (flooridx is not null)
                {
                    List<RoomTb>? model = await context.RoomTbs.Where(m => m.DelYn != true && m.FloorTbId == flooridx).ToListAsync();

                    if (model is [_, ..])
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    

        /// <summary>
        /// 공간 인덱스로 공간 검색
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async ValueTask<RoomTb?> GetRoomInfo(int? roomidx)
        {
            try
            {
                if(roomidx is not null)
                {
                    RoomTb? model = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == roomidx);

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


    }
}
