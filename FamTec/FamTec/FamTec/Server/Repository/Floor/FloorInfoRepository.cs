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
                    .AnyAsync(r => r.FloorTbId == floorTbId && r.DelYn != true)
                    .ConfigureAwait(false);

                return RoomCheck;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                await context.FloorTbs.AddAsync(model).ConfigureAwait(false);

                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteFloorInfo(List<int> roomidx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        foreach (int roomid in roomidx)
                        {
                            FloorTb? floortb = await context.FloorTbs
                            .FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true)
                            .ConfigureAwait(false);

                            if (floortb is not null)
                            {
                                floortb.DelYn = true;
                                floortb.DelDt = ThisDate;
                                floortb.DelUser = deleter;

                                context.FloorTbs.Update(floortb);
                            }
                            else
                            {
                                // 값이 없으면 잘못됨 roolback
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                        }
                        bool FloorResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (FloorResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            // 업데이트 실패시 롤백
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
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
                    .FirstOrDefaultAsync(m => m.Id == flooridx && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                    .ToListAsync()
                    .ConfigureAwait(false);

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
                throw;
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
                
                List<FloorTb>? floor = await context.FloorTbs
                    .Where(m => m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (floor is null || !floor.Any())
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
                throw;
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
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }
    
    }
}
