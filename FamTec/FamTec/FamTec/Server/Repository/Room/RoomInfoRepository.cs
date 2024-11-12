using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Room
{
    public class RoomInfoRepository : IRoomInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<RoomInfoRepository> CreateBuilderLogger;

        public RoomInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<RoomInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false반환
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool?> DelRoomCheck(int roomid)
        {
            try
            {
                bool FacilityCheck = await context.FacilityTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true).ConfigureAwait(false);
                bool InventoryCheck = await context.InventoryTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true).ConfigureAwait(false);
                bool MaterialCheck = await context.MaterialTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true).ConfigureAwait(false);
                //bool StoreCheck = await context.StoreTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true).ConfigureAwait(false);
                bool UseMaintenenceMartialCheck = await context.UseMaintenenceMaterialTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true).ConfigureAwait(false);

                return FacilityCheck || InventoryCheck || MaterialCheck || UseMaintenenceMartialCheck;
                //return FacilityCheck || InventoryCheck || MaterialCheck || StoreCheck || UseMaintenenceMartialCheck;
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
        /// 공간정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<RoomTb?> AddAsync(RoomTb model)
        {
            try
            {
                await context.RoomTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 층에 해당하는 공간 List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<RoomTb>?> GetRoomList(int flooridx)
        {
            try
            {
                List<RoomTb>? model = await context.RoomTbs
                    .Where(m => m.DelYn != true && m.FloorTbId == flooridx)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
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
        /// 층 List에 해당하는 공간 List 정보 조회
        /// </summary>
        /// <param name="FloorList"></param>
        /// <returns></returns>
        public async Task<List<RoomTb>?> GetFloorRoomList(List<FloorTb> FloorList)
        {
            try
            {
                // 층 INDEX 뽑아냄
                List<int> FloorIdx = FloorList.Select(m => m.Id)
                    .ToList();

                List<RoomTb>? RoomList = await context.RoomTbs
                    .Where(m => FloorIdx.Contains(m.FloorTbId) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (RoomList is [_, ..])
                    return RoomList;
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
        /// 공간 인덱스로 공간 검색
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async Task<RoomTb?> GetRoomInfo(int roomidx)
        {
            try
            {
                RoomTb? model = await context.RoomTbs
                    .FirstOrDefaultAsync(m => m.Id == roomidx && m.DelYn != true)
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
        /// 공간정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
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
        /// 공간정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
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
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteRoomInfo(List<int> idx, string deleter)
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
                        if (idx is null || !idx.Any())
                            return (bool?)null;

                        foreach (int roomid in idx)
                        {
                            RoomTb? RoomTB = await context.RoomTbs
                                .FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true)
                                .ConfigureAwait(false);

                            if (RoomTB is not null)
                            {
                                RoomTB.DelYn = true;
                                RoomTB.DelDt = ThisDate;
                                RoomTB.DelUser = deleter;

                                context.RoomTbs.Update(RoomTB);
                            }
                            else
                            {
                                // 공간정보 없음 -- 데이터가 잘못됨 (롤백)
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                        }

                        bool UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (UpdateResult) // 성공시 커밋
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else // 실패시 롤백
                        {
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
        /// 공간 삭제 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<bool?> RoomDeleteCheck(int placeid, int roomidx)
        {
            try
            {
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid &&
                    m.RoomTbId == roomidx)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..]) // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 false
                    return false;
                else // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 true
                    return true;
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
        /// 사업장에 해당하는 전체건물 - 전체층 - 전체공간 Group
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<PlaceRoomListDTO>?> GetPlaceAllGroupRoomInfo(int placeid)
        {
            try
            {
                List<BuildingTb>? BuildingList = await context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync().ConfigureAwait(false);
                if (BuildingList is null || !BuildingList.Any())
                    return null;

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => BuildingList.Select(b => b.Id).Contains(m.BuildingTbId) && m.DelYn != true).ToListAsync().ConfigureAwait(false);
                if (FloorList is null || !FloorList.Any())
                    return null;

                List<RoomTb>? RoomList = await context.RoomTbs.Where(m => FloorList.Select(f => f.Id).Contains(m.FloorTbId) && m.DelYn != true).ToListAsync().ConfigureAwait(false);
                if (RoomList is null || !RoomList.Any())
                    return null;

                #region 수정
                List<PlaceRoomListDTO>? model = BuildingList
                .Select(building => new PlaceRoomListDTO
                {
                    Id = building.Id,
                    Name = building.Name,
                    FloorList = FloorList
                        .Where(floor => floor.BuildingTbId == building.Id && RoomList.Any(room => room.FloorTbId == floor.Id))
                        .Select(floor => new BuildingFloor
                        {
                            Id = floor.Id,
                            Name = floor.Name,
                            RoomList = RoomList
                                .Where(room => room.FloorTbId == floor.Id)
                                .Select(room => new FloorRoom
                                {
                                    Id = room.Id,
                                    Name = room.Name
                                }).ToList()
                        })
                        .Where(floor => floor.RoomList.Any()) // RoomList가 비어 있지 않은 Floor만 선택
                        .ToList()
                })
                .Where(building => building.FloorList.Any()) // FloorList가 비어 있지 않은 Building만 선택
                .ToList();

                            if (model is [_, ..])
                                return model;
                            else
                                return null;
                #endregion
                /*
                List<PlaceRoomListDTO>? model = BuildingList.Select(building => new PlaceRoomListDTO
                {
                    Id = building.Id,
                    Name = building.Name,
                    FloorList = FloorList.Where(floor => floor.BuildingTbId == building.Id)
                         .Select(floor => new BuildingFloor
                         {
                             Id = floor.Id,
                             Name = floor.Name,
                             RoomList = RoomList.Where(room => room.FloorTbId == floor.Id)
                                                .Select(room => new FloorRoom
                                                {
                                                    Id = room.Id,
                                                    Name = room.Name
                                                }).ToList()
                         }).ToList()
                }).ToList();

                if (model is [_, ..])
                    return model;
                else
                    return null;
                */
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
        /// 사업장에 해당하는 전체 공간 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<RoomTb>?> GetPlaceAllRoomList(int placeid)
        {
            try
            {
                List<RoomTb>? RoomTB = await (from PlaceInfo in context.PlaceTbs.Where(m => m.Id == placeid && m.DelYn != true)
                            join BuildingInfo in context.BuildingTbs.Where(m => m.DelYn != true)
                            on PlaceInfo.Id equals BuildingInfo.PlaceTbId
                            join FloorInfo in context.FloorTbs.Where(m => m.DelYn != true)
                            on BuildingInfo.Id equals FloorInfo.BuildingTbId
                            join RoomInfo in context.RoomTbs.Where(m => m.DelYn != true)
                            on FloorInfo.Id equals RoomInfo.FloorTbId
                            select RoomInfo).ToListAsync();

                if (RoomTB is [_, ..])
                    return RoomTB;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
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
