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

        /// <summary>
        /// 공간정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateRoomInfo(RoomTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.RoomTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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
        /// 공간정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteRoomInfo(RoomTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.RoomTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteRoomInfo(List<int>? idx, string? deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(deleter))
                        return null;

                    if (idx is [_, ..])
                    {
                        foreach(int roomid in idx)
                        {
                            RoomTb? RoomTB = await context.RoomTbs.FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);
                            if(RoomTB is not null)
                            {
                                RoomTB.DelYn = true;
                                RoomTB.DelDt = DateTime.Now;
                                RoomTB.DelUser = deleter;

                                context.RoomTbs.Update(RoomTB);
                                bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!UpdateResult) // 업데이트 실패시 롤백
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }
                            }
                            else
                            {
                                // 공간정보 없음 -- 데이터가 잘못됨 (롤백)
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }


        /// <summary>
        /// 공간 삭제 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> RoomDeleteCheck(int? placeid, int? roomidx)
        {
            try
            {
                if (placeid is null)
                {
                    return null;
                }
                if (roomidx is null)
                {
                    return null;
                }

                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid &&
                    m.RoomTbId == roomidx)
                    .ToListAsync();

                if (model is [_, ..]) // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 false
                    return false;
                else // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 true
                    return true;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

  
    }
}
