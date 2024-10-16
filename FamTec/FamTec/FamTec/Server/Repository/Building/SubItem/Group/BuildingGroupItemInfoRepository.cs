using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Building.SubItem.Group
{
    public class BuildingGroupItemInfoRepository : IBuildingGroupItemInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingGroupItemInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BuildingItemGroupTb?> AddAsync(BuildingItemGroupTb model)
        {
            try
            {
                await context.BuildingItemGroupTbs.AddAsync(model).ConfigureAwait(false);

                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch (DbUpdateException dbEx)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                throw;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 그룹리스트 상세검색 buildingid로 검색
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async Task<List<BuildingItemGroupTb>?> GetAllGroupList(int buildingid)
        {
            try
            {
                List<BuildingItemGroupTb>? model = await context.BuildingItemGroupTbs
                    .Where(m => m.BuildingTbId == buildingid && 
                                m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
                    return null;  
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 그룹 상세검색 groupid로 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async Task<BuildingItemGroupTb?> GetGroupInfo(int groupid)
        {
            try
            {
                BuildingItemGroupTb? model = await context.BuildingItemGroupTbs
                    .FirstOrDefaultAsync(m => m.Id == groupid && 
                                              m.DelYn != true)
                    .ConfigureAwait(false); ;

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 그룹수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateGroupInfo(BuildingItemGroupTb model)
        {
            try
            {
                context.BuildingItemGroupTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException dbEx)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                throw;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteGroupInfo(BuildingItemGroupTb model)
        {
            try
            {
                context.BuildingItemGroupTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException dbEx)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                throw;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 건물에 그룹 한번에 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int> AddGroupAsync(List<AddGroupDTO> dto, string creater, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool SaveResult = false;
            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // GROUP INSERT
                        foreach(AddGroupDTO Group in dto)
                        {
                            BuildingTb? BuildingTB = await context.BuildingTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.PlaceTbId == placeid && m.Id == Group.BuildingIdx).ConfigureAwait(false);
                            
                            if (BuildingTB is null)
                                return -1; // 잘못된 요청임.

                            BuildingItemGroupTb GroupTB = new BuildingItemGroupTb();
                            GroupTB.Name = Group.Name!.ToString(); // 그룹의 명칭
                            GroupTB.CreateDt = ThisDate; // 현재시간
                            GroupTB.CreateUser = creater; // 생성자
                            GroupTB.UpdateDt = ThisDate; // 현재시간
                            GroupTB.UpdateUser = creater; // 생성자
                            GroupTB.BuildingTbId = Group.BuildingIdx!.Value; // 건물 ID

                            await context.BuildingItemGroupTbs.AddAsync(GroupTB).ConfigureAwait(false);
                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if(!SaveResult)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션
                                return 0;
                            }


                            if (Group.AddGroupKey is [_, ..])
                            {
                                // KEY INSERT
                                foreach (AddGroupItemKeyDTO Key in Group.AddGroupKey)
                                {
                                    BuildingItemKeyTb KeyTB = new BuildingItemKeyTb();
                                    KeyTB.Name = Key.Name!.ToString(); // 키의 명칭
                                    KeyTB.Unit = Key.Unit!.ToString(); // 키의 단위
                                    KeyTB.BuildingGroupTbId = GroupTB.Id; // 상위 그룹 ID
                                    KeyTB.CreateDt = ThisDate; // 현재시간
                                    KeyTB.CreateUser = creater; // 생성자
                                    KeyTB.UpdateDt = ThisDate; // 현재시간
                                    KeyTB.UpdateUser = creater; // 생성자

                                    await context.BuildingItemKeyTbs.AddAsync(KeyTB).ConfigureAwait(false);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션
                                        return 0;
                                    }

                                    if (Key.ItemValues is [_, ..])
                                    {
                                        foreach (AddGroupItemValueDTO Value in Key.ItemValues)
                                        {
                                            BuildingItemValueTb ValueTB = new BuildingItemValueTb();
                                            ValueTB.ItemValue = Value.Values!.ToString(); // 값
                                            ValueTB.CreateDt = ThisDate;
                                            ValueTB.CreateUser = creater;
                                            ValueTB.UpdateDt = ThisDate;
                                            ValueTB.UpdateUser = creater;
                                            ValueTB.BuildingKeyTbId = KeyTB.Id;

                                            await context.BuildingItemValueTbs.AddAsync(ValueTB).ConfigureAwait(false);
                                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                            if(!SaveResult)
                                            {
                                                await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션
                                                return 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // 모든 작업이 성공적으로 완료된 후 트랜잭션 커밋
                        await transaction.CommitAsync().ConfigureAwait(false);
                        return 1;
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
        /// 그룹삭제 - 2 Transaction
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteGroupInfo(int groupid, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        BuildingItemGroupTb? GroupTB = await context.BuildingItemGroupTbs
                            .FirstOrDefaultAsync(m => m.Id == groupid)
                            .ConfigureAwait(false);

                        if (GroupTB is null)
                            return (bool?)null;

                        GroupTB.DelDt = ThisDate;
                        GroupTB.DelUser = deleter;
                        GroupTB.DelYn = true;

                        context.BuildingItemGroupTbs.Update(GroupTB);

                        List<BuildingItemKeyTb>? KeyTB = await context.BuildingItemKeyTbs
                            .Where(m => m.BuildingGroupTbId == groupid)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        if (KeyTB is [_, ..])
                        {
                            foreach (BuildingItemKeyTb KeyModel in KeyTB)
                            {
                                KeyModel.DelDt = ThisDate;
                                KeyModel.DelUser = deleter;
                                KeyModel.DelYn = true;

                                context.BuildingItemKeyTbs.Update(KeyModel);

                                List<BuildingItemValueTb>? ValueTB = await context.BuildingItemValueTbs
                                .Where(m => m.BuildingKeyTbId == KeyModel.Id)
                                .ToListAsync()
                                .ConfigureAwait(false);

                                if (ValueTB is [_, ..])
                                {
                                    foreach (BuildingItemValueTb ValueModel in ValueTB)
                                    {
                                        ValueModel.DelDt = ThisDate;
                                        ValueModel.DelUser = deleter;
                                        ValueModel.DelYn = true;

                                        context.BuildingItemValueTbs.Update(ValueModel);
                                    }
                                }
                            }
                        }

                        bool DeleteResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (DeleteResult)
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
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException dbEx)
                    {
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                        throw;
                    }
                    catch (MySqlException mysqlEx)
                    {
                        LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                        throw;
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
        /// 넘어온 Id에 포함되어있는 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async Task<List<BuildingItemGroupTb>?> ContainsGroupList(List<int> GroupId, int buildingid)
        {
            try
            {
                List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs
                    .Where(e => GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (grouptb is [_, ..])
                    return grouptb;
                else
                    return null;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 넘어온 Id에 포함되어있지 않은 GroupTb 반환
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<List<BuildingItemGroupTb>?> NotContainsGroupList(List<int> GroupId, int buildingid)
        {
            try
            {
                List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs
                    .Where(e => !GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (grouptb is [_, ..])
                    return grouptb;
                else
                    return null;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
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
