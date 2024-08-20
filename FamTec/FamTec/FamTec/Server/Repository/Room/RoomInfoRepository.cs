using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Room
{
    public class RoomInfoRepository : IRoomInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public RoomInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 공간정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<RoomTb?> AddAsync(RoomTb model)
        {
            try
            {
                context.RoomTbs.Add(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
                {
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
        public async ValueTask<List<RoomTb>?> GetRoomList(int flooridx)
        {
            try
            {
                List<RoomTb>? model = await context.RoomTbs
                    .Where(m => m.DelYn != true && m.FloorTbId == flooridx)
                    .ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 층 List에 해당하는 공간 List 정보 조회
        /// </summary>
        /// <param name="FloorList"></param>
        /// <returns></returns>
        public async ValueTask<List<RoomTb>?> GetFloorRoomList(List<FloorTb> FloorList)
        {
            try
            {
                // 층 INDEX 뽑아냄
                List<int> FloorIdx = FloorList.Select(m => m.Id)
                    .ToList();

                List<RoomTb>? RoomList = await context.RoomTbs
                    .Where(m => FloorIdx.Contains(m.FloorTbId) && m.DelYn != true)
                    .ToListAsync();

                if (RoomList is [_, ..])
                    return RoomList;
                else
                    return null;
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
        public async ValueTask<RoomTb?> GetRoomInfo(int roomidx)
        {
            try
            {
                RoomTb? model = await context.RoomTbs
                    .FirstOrDefaultAsync(m => m.Id == roomidx && m.DelYn != true);

                if (model is not null)
                    return model;
                else
                    return null;
               
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
        public async ValueTask<bool?> UpdateRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
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
        public async ValueTask<bool?> DeleteRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
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
        public async ValueTask<bool?> DeleteRoomInfo(List<int> idx, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (idx is [_, ..])
                    {
                        foreach(int roomid in idx)
                        {
                            RoomTb? RoomTB = await context.RoomTbs
                                .FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);

                            if(RoomTB is not null)
                            {
                                RoomTB.DelYn = true;
                                RoomTB.DelDt = DateTime.Now;
                                RoomTB.DelUser = deleter;

                                context.RoomTbs.Update(RoomTB);
                            }
                            else
                            {
                                // 공간정보 없음 -- 데이터가 잘못됨 (롤백)
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }

                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (UpdateResult) // 성공시 커밋
                        {
                            await transaction.CommitAsync();
                            return true;
                        }
                        else // 실패시 롤백
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
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
        public async ValueTask<bool?> RoomDeleteCheck(int placeid, int roomidx)
        {
            try
            {
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
