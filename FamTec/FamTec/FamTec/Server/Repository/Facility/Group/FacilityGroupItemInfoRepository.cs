using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Facility.Group
{
    public class FacilityGroupItemInfoRepository : IFacilityGroupItemInfoRepository
    {
        private readonly WorksContext context;
        private readonly ILogService LogService;
        private readonly ILogger<FacilityGroupItemInfoRepository> BuilderLogger;

        public FacilityGroupItemInfoRepository(WorksContext _context,
            ILogService _logservice,
            ILogger<FacilityGroupItemInfoRepository> _builderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        /// <summary>
        /// ASP - 빌드로그
        /// </summary>
        /// <param name="ex"></param>
        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FacilityItemGroupTb?> AddAsync(FacilityItemGroupTb model)
        {
            try
            {
                await context.FacilityItemGroupTbs.AddAsync(model).ConfigureAwait(false);

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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 설비ID에 포함되어있는 그룹List 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public async Task<List<FacilityItemGroupTb>?> GetAllGroupList(int facilityId)
        {
            try
            {
                List<FacilityItemGroupTb>? model = await context.FacilityItemGroupTbs
                    .Where(m => m.FacilityTbId == facilityId && m.DelYn != true)
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 그룹ID로 모델 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async Task<FacilityItemGroupTb?> GetGroupInfo(int groupid)
        {
            try
            {
                FacilityItemGroupTb? model = await context.FacilityItemGroupTbs
                    .FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true)
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateGroupInfo(FacilityItemGroupTb model)
        {
            try
            {
                context.FacilityItemGroupTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteGroupInfo(FacilityItemGroupTb model)
        {
            try
            {
                context.FacilityItemGroupTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 설비그룹 한번에 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int> AddGroupAsync(List<AddGroupDTO> dto, string creater)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool SaveResult = false;
            DateTime ThisDate = DateTime.Now;

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
                            FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == Group.Id).ConfigureAwait(false);

                            if (FacilityTB is null)
                                return -1; // 잘못된 요청임

                            FacilityItemGroupTb GroupTB = new FacilityItemGroupTb();
                            GroupTB.Name = Group.Name!.ToString(); // 그룹의 명칭
                            GroupTB.CreateDt = ThisDate; // 현재시간
                            GroupTB.CreateUser = creater; // 생성자
                            GroupTB.UpdateDt = ThisDate; // 현재식나
                            GroupTB.UpdateUser = creater; // 생성자
                            GroupTB.FacilityTbId = Group.Id!.Value; // 설비 ID

                            await context.FacilityItemGroupTbs.AddAsync(GroupTB).ConfigureAwait(false);
                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if(!SaveResult)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션
                                return 0;
                            }

                            if(Group.AddGroupKey is [_, ..])
                            {
                                // KEY INSERT
                                foreach(AddGroupItemKeyDTO Key in Group.AddGroupKey)
                                {
                                    FacilityItemKeyTb KeyTB = new FacilityItemKeyTb();
                                    KeyTB.Name = Key.Name!.ToString(); // 키의 명칭
                                    KeyTB.Unit = Key.Unit!.ToString(); // 키의 단위
                                    KeyTB.FacilityItemGroupTbId = GroupTB.Id; // 상위 그룹 ID
                                    KeyTB.CreateDt = ThisDate; // 현재시간
                                    KeyTB.CreateUser = creater; // 생성자
                                    KeyTB.UpdateDt = ThisDate; // 현재시간
                                    KeyTB.UpdateUser = creater; // 생성자

                                    await context.FacilityItemKeyTbs.AddAsync(KeyTB).ConfigureAwait(false);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if(!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션
                                        return 0;
                                    }

                                    if(Key.ItemValues is [_, ..])
                                    {
                                        foreach(AddGroupItemValueDTO Value in Key.ItemValues)
                                        {
                                            FacilityItemValueTb ValueTB = new FacilityItemValueTb();
                                            ValueTB.ItemValue = Value.Values!.ToString(); // 값
                                            ValueTB.CreateDt = ThisDate; // 현재시간
                                            ValueTB.CreateUser = creater;
                                            ValueTB.UpdateDt = ThisDate; // 현재시간
                                            ValueTB.UpdateUser = creater;
                                            ValueTB.FacilityItemKeyTbId = KeyTB.Id;

                                            await context.FacilityItemValueTbs.AddAsync(ValueTB).ConfigureAwait(false);
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
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 그룹삭제 -2
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
                // 강제로 디버깅 포인트잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        FacilityItemGroupTb? GroupTB = await context.FacilityItemGroupTbs
                            .FirstOrDefaultAsync(m => m.Id == groupid)
                            .ConfigureAwait(false);

                        if (GroupTB is null)
                            return (bool?)null;

                        GroupTB.DelDt = ThisDate;
                        GroupTB.DelUser = deleter;
                        GroupTB.DelYn = true;

                        context.FacilityItemGroupTbs.Update(GroupTB);

                        List<FacilityItemKeyTb>? KeyTB = await context.FacilityItemKeyTbs
                        .Where(m => m.FacilityItemGroupTbId == groupid)
                        .ToListAsync()
                        .ConfigureAwait(false);

                        if (KeyTB is [_, ..])
                        {
                            foreach (FacilityItemKeyTb KeyModel in KeyTB)
                            {
                                KeyModel.DelDt = ThisDate;
                                KeyModel.DelUser = deleter;
                                KeyModel.DelYn = true;

                                context.FacilityItemKeyTbs.Update(KeyModel);

                                List<FacilityItemValueTb>? ValueTB = await context.FacilityItemValueTbs
                                .Where(m => m.FacilityItemKeyTbId == KeyModel.Id)
                                .ToListAsync()
                                .ConfigureAwait(false);

                                if (ValueTB is [_, ..])
                                {
                                    foreach (FacilityItemValueTb ValueModel in ValueTB)
                                    {
                                        ValueModel.DelDt = ThisDate;
                                        ValueModel.DelUser = deleter;
                                        ValueModel.DelYn = true;

                                        context.FacilityItemValueTbs.Update(ValueModel);
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
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                }
            });
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
