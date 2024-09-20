using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace FamTec.Server.Repository.Floor
{
    public class FloorInfoRepository : IFloorInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FloorInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        // 삭제가능여부 체크
        // 참조하는게 하나라도 있으면 true 반환
        // 아니면 false 반환
        public async ValueTask<bool?> DelFloorCheck(int floorTbId)
        {
            try
            {
                // 참조된 테이블이 하나라도 있으면 true 반환
                bool RoomCheck = await context.RoomTbs
                    .AnyAsync(r => r.FloorTbId == floorTbId && r.DelYn != true);

                return RoomCheck;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
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
                await context.FloorTbs.AddAsync(model);
                
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                
                if (AddResult)
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
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (int roomid in roomidx)
                    {
                        FloorTb? floortb = await context.FloorTbs.FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);
                        if (floortb is not null)
                        {
                            floortb.DelYn = true;
                            floortb.DelDt = DateTime.Now;
                            floortb.DelUser = deleter;

                            context.FloorTbs.Update(floortb);
                        }
                        else
                        {
                            // 값이 없으면 잘못됨 roolback
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }
                    bool FloorResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if (FloorResult)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        // 업데이트 실패시 롤백
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
#region 수정전
            //            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            //            bool? result = await strategy.ExecuteAsync(async () =>
            //            {
            //#if DEBUG
            //                // 디버깅 포인트를 강제로 잡음.
            //                Debugger.Break();
            //#endif
            //                using (var transaction = await context.Database.BeginTransactionAsync())
            //                {
            //                    try
            //                    {
            //                        foreach (int roomid in roomidx)
            //                        {
            //                            FloorTb? floortb = await context.FloorTbs.FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);
            //                            if (floortb is not null)
            //                            {
            //                                floortb.DelYn = true;
            //                                floortb.DelDt = DateTime.Now;
            //                                floortb.DelUser = deleter;

            //                                context.FloorTbs.Update(floortb);
            //                            }
            //                            else
            //                            {
            //                                // 값이 없으면 잘못됨 roolback
            //                                await transaction.RollbackAsync();
            //                                return false;
            //                            }
            //                        }
            //                        bool FloorResult = await context.SaveChangesAsync() > 0 ? true : false;
            //                        if (FloorResult)
            //                        {
            //                            await transaction.CommitAsync();
            //                            return true;
            //                        }
            //                        else
            //                        {
            //                            // 업데이트 실패시 롤백
            //                            await transaction.RollbackAsync();
            //                            return false;
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        LogService.LogMessage(ex.ToString());
            //                        throw new ArgumentNullException();
            //                    }
            //                }
            //            });
            //            return result;
            #endregion
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

                if (model is not null && model.Any())
                {
                    List<FloorTb> sortedData = model.OrderBy(item => item.Name, new CustomComparer()).ToList();
                    return sortedData;
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
                if(floor is null || !floor.Any())
                    return null;

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

                if (result is not null && result.Any())
                {
                    return result;
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
