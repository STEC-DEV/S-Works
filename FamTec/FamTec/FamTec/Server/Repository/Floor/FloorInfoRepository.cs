using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Floor
{
    public class FloorInfoRepository : IFloorInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FloorInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 층추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FloorTb?> AddAsync(FloorTb model)
        {
            try
            {
                context.FloorTbs.Add(model);
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
        /// 층삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteFloorInfo(FloorTb model)
        {
            try
            {
                context.FloorTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteFloorInfo(List<int> roomidx, string deleter)
        {
            using(var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int roomid in roomidx)
                    {
                        FloorTb? floortb = await context.FloorTbs.FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);
                        if(floortb is not null)
                        {
                            floortb.DelYn = true;
                            floortb.DelDt = DateTime.Now;
                            floortb.DelUser = deleter;

                            context.FloorTbs.Update(floortb);
                            bool FloorResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (!FloorResult)
                            {
                                // 업데이트 실패시 롤백
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                        else
                        {
                            // 값이 없으면 잘못됨 roolback
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 층 인덱스로 층 검색
        /// </summary>
        /// <param name="flooridx"></param>
        /// <returns></returns>
        public async ValueTask<FloorTb?> GetFloorInfo(int flooridx)
        {
            try
            {
                FloorTb? model = await context.FloorTbs
                    .FirstOrDefaultAsync(m => m.Id == flooridx && m.DelYn != true);

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
        /// 건물에 해당하는 층 리스트 조회
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async ValueTask<List<FloorTb>?> GetFloorList(int buildingtbid)
        {
            try
            {
                List<FloorTb>? model = await context.FloorTbs
                    .Where(m => m.BuildingTbId == buildingtbid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
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
        /// 건물에 해당하는 층List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<FloorTb>?> GetFloorList(List<BuildingTb> model)
        {
            try
            {
                
                List<FloorTb>? floor = await context.FloorTbs.Where(m => m.DelYn != true).ToListAsync();

                if (floor is [_, ..])
                {
                    List<FloorTb>? result = (from bdtb in model
                                                join floortb in floor
                                                on bdtb.Id equals floortb.BuildingTbId
                                                where floortb.DelYn != true && bdtb.DelYn != true
                                                select new FloorTb
                                                {
                                                    Id = floortb.Id,
                                                    Name = floortb.Name,
                                                    CreateDt = floortb.CreateDt,
                                                    CreateUser = floortb.CreateUser,
                                                    UpdateDt = floortb.UpdateDt,
                                                    UpdateUser = floortb.UpdateUser,
                                                    DelYn = floortb.DelYn,
                                                    DelDt = floortb.DelDt,
                                                    DelUser = floortb.DelUser,
                                                    BuildingTbId = floortb.BuildingTbId
                                                }).OrderBy(m => m.CreateDt)
                                                .ToList();

                    if (result is [_, ..])
                        return result;
                    else
                        return null;
                }
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
        /// 층 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateFloorInfo(FloorTb model)
        {
            try
            {
                context.FloorTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
