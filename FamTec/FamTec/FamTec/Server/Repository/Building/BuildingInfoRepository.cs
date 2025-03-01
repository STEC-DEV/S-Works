﻿using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Building
{
    public class BuildingInfoRepository : IBuildingInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<BuildingInfoRepository> CreateBuilderLogger;

        public BuildingInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<BuildingInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async Task<bool?> DelBuildingCheck(int buildingid)
        {
            try
            {
                bool FloorCheck = await context.FloorTbs.AnyAsync(m => m.BuildingTbId == buildingid && m.DelYn != true).ConfigureAwait(false);
                
                return FloorCheck;
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
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int?> TotalBuildingCount(int placeid)
        {
            try
            {
                int totalCount = await context.BuildingTbs
                    .CountAsync(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .ConfigureAwait(false);

                return totalCount;
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
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BuildingTb?> AddAsync(BuildingTb model)
        {
            try
            {
                await context.BuildingTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 건물 여러개 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddBuildingList(List<BuildingTb> model)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        await context.BuildingTbs.AddRangeAsync(model).ConfigureAwait(false);

                        bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(AddResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
                    }
                    catch(Exception ex) when(IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch(DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch(MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 해당사업장 건물 전체조회
        /// </summary>
        /// <param name="placeid">로그인한 사업장정보</param>
        /// <returns></returns>
        public async Task<List<BuildingTb>?> GetAllBuildingList(int placeid)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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
        /// 해당 사업장의 건물조회 - 페이지네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<BuildingTb>?> GetAllBuildingPageList(int placeid, int skip, int take)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip(skip) // 건너뛸 개수
                    .Take(take) // 출력할 개수
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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
        /// 빌딩 인덱스로 빌딩 검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<BuildingTb?> GetBuildingInfo(int buildingId)
        {
            try
            {
                BuildingTb? model = await context.BuildingTbs
                    .FirstOrDefaultAsync(m => m.Id == buildingId && 
                                              m.DelYn != true)
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
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateBuildingInfo(BuildingTb model)
        {
            try
            {
                context.BuildingTbs.Update(model);
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
        /// 삭제할 리스트 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<BuildingTb>?> GetDeleteList(List<int> buildingId)
        {
            try
            {
                List<BuildingTb>? buildinglist = await context.BuildingTbs
                                .Where(m => buildingId.Contains(m.Id) && 
                                            m.DelYn != true)
                                .ToListAsync()
                                .ConfigureAwait(false);

                if (buildinglist is [_, ..])
                {
                    // 층 Table의 DelYN = true
                    List<BuildingTb> BuildingTb = (from buildingtb in buildinglist
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn == true)
                                                    on buildingtb.Id equals floortb.BuildingTbId
                                                    select buildingtb).ToList();
                   

                    // 건물 Table의 층이 할당안된 테이블
                    List<BuildingTb>? deleteList = buildinglist.Where(a => !context.FloorTbs.Any(b => b.BuildingTbId == a.Id)).ToList();
                        
                    // Merge
                    deleteList.AddRange(BuildingTb);

                    if (deleteList is [_, ..])
                        return deleteList;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        /// 제네릭에 해당하는 빌딩정보들 반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async Task<List<BuildingTb>?> GetBuildings(List<int> buildingid)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => buildingid.Contains(m.Id))
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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

        public async Task<bool?> DeleteBuildingInfo(BuildingTb model)
        {
            try
            {
                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                model.BuildingCd = $"{model.BuildingCd}_{model.Id}";
                context.BuildingTbs.Update(model);
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
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<BuildingTb>?> SelectPlaceBuildingList(List<int> placeidx)
        {
            try
            {
                List<BuildingTb>? buildingtb = await context.BuildingTbs
                    .Where(m => placeidx.Contains(Convert.ToInt32(m.PlaceTbId)) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (buildingtb is not null && buildingtb.Any())
                    return buildingtb;
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
        /// 건물정보 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteBuildingList(List<int> buildingid, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool UpdateResult = false;

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        if (buildingid is null || !buildingid.Any())
                            return (bool?)null;

                        foreach (int bdid in buildingid)
                        {
                            BuildingTb? BuildingTB = await context.BuildingTbs
                                .FirstOrDefaultAsync(m => m.Id == bdid && m.DelYn != true)
                                .ConfigureAwait(false);

                            if (BuildingTB is not null)
                            {
                                BuildingTB.DelYn = true;
                                BuildingTB.DelDt = ThisDate;
                                BuildingTB.DelUser = deleter;

                                context.BuildingTbs.Update(BuildingTB);
                                UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!UpdateResult)
                                {
                                    // 업데이트 실패시 롤백
                                    await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                    return false;
                                }

                                List<VocTb>? VocList = await context.VocTbs
                                .Where(m => m.BuildingTbId == BuildingTB.Id && m.DelYn != true)
                                .ToListAsync()
                                .ConfigureAwait(false);

                                if(VocList is [_, ..])
                                {
                                    foreach(VocTb VocTB in VocList)
                                    {
                                        VocTB.DelYn = true;
                                        VocTB.DelDt = ThisDate;
                                        VocTB.DelUser = deleter;

                                        context.VocTbs.Update(VocTB);

                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if(!UpdateResult)
                                        {
                                            await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                            return false;
                                        }

                                        List<CommentTb>? CommentList = await context.CommentTbs.Where(m => m.DelYn != true && m.VocTbId == VocTB.Id).ToListAsync();
                                        if(CommentList is [_, ..])
                                        {
                                            foreach(CommentTb CommentTB in CommentList)
                                            {
                                                CommentTB.DelYn = true;
                                                CommentTB.DelDt = ThisDate;
                                                CommentTB.DelUser = deleter;

                                                context.CommentTbs.Update(CommentTB);

                                                UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                if(!UpdateResult)
                                                {
                                                    await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }


                                List<BuildingItemGroupTb>? GroupList = await context.BuildingItemGroupTbs
                                    .Where(m => m.BuildingTbId == BuildingTB.Id && m.DelYn != true)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                                if (GroupList is [_, ..])
                                {
                                    foreach (BuildingItemGroupTb GroupTB in GroupList)
                                    {
                                        GroupTB.DelYn = true;
                                        GroupTB.DelDt = ThisDate;
                                        GroupTB.DelUser = deleter;

                                        context.BuildingItemGroupTbs.Update(GroupTB);
                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if (!UpdateResult)
                                        {
                                            // 업데이트 실패시 롤백
                                            await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                            return false;
                                        }
                                        List<BuildingItemKeyTb>? KeyList = await context.BuildingItemKeyTbs
                                            .Where(m => m.BuildingGroupTbId == GroupTB.Id && m.DelYn != true)
                                            .ToListAsync()
                                            .ConfigureAwait(false);

                                        if (KeyList is [_, ..])
                                        {
                                            foreach (BuildingItemKeyTb KeyTB in KeyList)
                                            {
                                                KeyTB.DelYn = true;
                                                KeyTB.DelDt = ThisDate;
                                                KeyTB.DelUser = deleter;

                                                context.BuildingItemKeyTbs.Update(KeyTB);
                                                UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                if (!UpdateResult)
                                                {
                                                    // 업데이트 실패시 롤백
                                                    await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                                    return false;
                                                }

                                                List<BuildingItemValueTb>? ValueList = await context.BuildingItemValueTbs
                                                .Where(m => m.BuildingKeyTbId == KeyTB.Id && m.DelYn != true)
                                                .ToListAsync()
                                                .ConfigureAwait(false);

                                                if (ValueList is [_, ..])
                                                {
                                                    foreach (BuildingItemValueTb ValueTB in ValueList)
                                                    {
                                                        ValueTB.DelYn = true;
                                                        ValueTB.DelDt = ThisDate;
                                                        ValueTB.DelUser = deleter;

                                                        context.BuildingItemValueTbs.Update(ValueTB);
                                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                        if (!UpdateResult)
                                                        {
                                                            // 업데이트 실패시 롤백
                                                            await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                                            return false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                return (bool?)null;
                            }
                        }

                        await transaction.CommitAsync().ConfigureAwait(false); // 커밋
                        
                        return true;
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

        public async Task<List<BuildingTb>?> GetPlaceAvailableBuildingList(int placeid, int materialid)
        {
            try
            {
                List<InventoryTb>? InventoryList = await context.InventoryTbs
                  .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.MaterialTbId == materialid)
                  .GroupBy(m => new { m.RoomTbId })  // RoomTbId로 그룹화
                  .Select(g => g.First())  // 각 그룹에서 첫 번째 InventoryTb 항목 선택
                  .ToListAsync()
                  .ConfigureAwait(false);

                if (InventoryList is null || !InventoryList.Any())
                    return null;

                //// RoomID 리스트로 만듬
                List<int>? RoomIdx = InventoryList.Select(m => m.RoomTbId).Distinct().ToList();
                

                List<RoomTb>? RoomList = await context.RoomTbs.Where(m => RoomIdx.Contains(m.Id) && m.DelYn != true).ToListAsync().ConfigureAwait(false);
                if(RoomList is null || !RoomList.Any())
                    return null;

                List<int>? RoomFloorList = RoomList.Select(m => m.FloorTbId).Distinct().ToList();

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => RoomFloorList.Contains(m.Id)).ToListAsync().ConfigureAwait(false);
                if (FloorList is null || !FloorList.Any())
                    return null;

                List<int>? FloorBuildingList = FloorList.Select(m => m.BuildingTbId).Distinct().ToList();

                List<BuildingTb> BuildingList = await context.BuildingTbs
                    .Where(m => FloorBuildingList.Contains(m.Id))
                    .GroupBy(m => new { m.Id})
                    .Select(g => g.First())
                    .ToListAsync()
                    .ConfigureAwait(false);

                return BuildingList;
            }
            catch(Exception ex)
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
