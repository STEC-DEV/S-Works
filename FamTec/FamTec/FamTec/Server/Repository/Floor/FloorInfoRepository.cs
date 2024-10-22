using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Floor
{
    public class FloorInfoRepository : IFloorInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<FloorInfoRepository> CreateBuilderLogger;

        public FloorInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<FloorInfoRepository> _createbuilderlogger
        )
        {
            this.context = _context;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        // 삭제가능여부 체크
        // 참조하는게 하나라도 있으면 true 반환
        // 아니면 false 반환
        public async Task<bool?> DelFloorCheck(int floorTbId)
        {
            try
            {
                // 참조된 테이블이 하나라도 있으면 true 반환
                bool RoomCheck = await context.RoomTbs
                    .AnyAsync(r => r.FloorTbId == floorTbId && r.DelYn != true)
                    .ConfigureAwait(false);

                return RoomCheck;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 층추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FloorTb?> AddAsync(FloorTb model)
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
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 층삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteFloorInfo(FloorTb model)
        {
            try
            {
                context.FloorTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteFloorInfo(List<int> roomidx, string deleter)
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
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
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
        public async Task<FloorTb?> GetFloorInfo(int flooridx)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 건물에 해당하는 층 리스트 조회
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async Task<List<FloorTb>?> GetFloorList(int buildingtbid)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 건물에 해당하는 층List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<FloorTb>?> GetFloorList(List<BuildingTb> model)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 층 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateFloorInfo(FloorTb model)
        {
            try
            {
                context.FloorTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 데드락 감지코드
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsDeadlockException(Exception ex)
        {
            // MySqlException 및 MariaDB의 교착 상태 오류 코드는 일반적으로 1213입니다.
            if (ex is MySqlException mysqlEx && mysqlEx.Number == 1213)
                return true;

            // InnerException에도 동일한 확인 로직을 적용
            if (ex.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1213)
                return true;

            return false;
        }
    }
}
