using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Facility.ItemKey
{
    public class FacilityItemKeyInfoRepository : IFacilityItemKeyInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<FacilityItemKeyInfoRepository> CreateBuilderLogger;

        public FacilityItemKeyInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<FacilityItemKeyInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 그룹의 Key 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FacilityItemKeyTb?> AddAsync(FacilityItemKeyTb model)
        {
            try
            {
                await context.FacilityItemKeyTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 그룹ID에 포함되어있는 KEY 리스트 전체 반환
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        public async Task<List<FacilityItemKeyTb>?> GetAllKeyList(int groupitemid)
        {
            try
            {
                List<FacilityItemKeyTb>? model = await context.FacilityItemKeyTbs
                    .Where(m => m.FacilityItemGroupTbId == groupitemid && m.DelYn != true)
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
        /// KeyID에 해당하는 KEY모델 반환
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async Task<FacilityItemKeyTb?> GetKeyInfo(int keyid)
        {
            try
            {
                FacilityItemKeyTb? model = await context.FacilityItemKeyTbs
                    .FirstOrDefaultAsync(m => m.Id == keyid && m.DelYn != true)
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
        /// Key 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateKeyInfo(FacilityItemKeyTb model)
        {
            try
            {
                context.FacilityItemKeyTbs.Update(model);
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
        /// 그룹 KEY - VALUE 업데이트
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateKeyInfo(UpdateKeyDTO dto, string updater)
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
                        FacilityItemKeyTb? KeyTB = await context.FacilityItemKeyTbs
                            .FirstOrDefaultAsync(m => m.Id == dto.ID && m.DelYn != true)
                            .ConfigureAwait(false);

                        if (KeyTB is null)
                            return (bool?)null;

                        KeyTB.Name = dto.Itemkey!;
                        KeyTB.Unit = dto.Unit;

                        context.FacilityItemKeyTbs.Update(KeyTB);
                        bool KeyUpdate = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                        if (!KeyUpdate)
                        {
                            // KEY 업데이트 실패시 Rollback
                            await transaction.RollbackAsync().ConfigureAwait(false); ;
                            return false;
                        }

                        // SELECT VALUE 정보 반환
                        List<FacilityItemValueTb>? ValueList = await context.FacilityItemValueTbs
                            .Where(m => m.FacilityItemKeyTbId == dto.ID && m.DelYn != true)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        // NULL 인값 INSERT OR UPDATE OR DELETE
                        if (dto.ValueList is [_, ..])
                        {
                            List<GroupValueListDTO> INSERTLIST = dto.ValueList.Where(m => m.ID == null).ToList();

                            // DTO IDList중 NULL이 아닌것 -- 수정대상
                            List<GroupValueListDTO> UPDATELIST = dto.ValueList.Where(m => m.ID != null).ToList();

                            // DB IDList
                            List<int> db_valueidx = ValueList.Select(m => m.Id).ToList();

                            List<int> updateidx = UPDATELIST.Select(m => m.ID!.Value).ToList();
                            // 삭제대상 (디비 인덱스 - DTO 인덱스 = 남는 DTO 인덱스)
                            List<int> delIdx = db_valueidx.Except(updateidx).ToList(); // list1에만 있는 값 -- DB에만 있는값 (삭제할값)

                            // 추가작업
                            foreach (GroupValueListDTO InsertInfo in INSERTLIST)
                            {
                                FacilityItemValueTb InsertTB = new FacilityItemValueTb();
                                InsertTB.ItemValue = InsertInfo.ItemValue!;
                                InsertTB.CreateDt = ThisDate;
                                InsertTB.CreateUser = updater;
                                InsertTB.UpdateDt = ThisDate;
                                InsertTB.UpdateUser = updater;
                                InsertTB.FacilityItemKeyTbId = dto.ID!.Value;
                                context.FacilityItemValueTbs.Add(InsertTB);
                            }

                            // 업데이트 작업
                            foreach (GroupValueListDTO UpdateInfo in UPDATELIST)
                            {
                                FacilityItemValueTb? UpdateTB = await context.FacilityItemValueTbs.
                                    FirstOrDefaultAsync(m => m.Id == UpdateInfo.ID && m.DelYn != true)
                                    .ConfigureAwait(false);

                                if (UpdateTB is not null)
                                {
                                    UpdateTB.ItemValue = UpdateInfo.ItemValue!;
                                    UpdateTB.UpdateDt = ThisDate;
                                    UpdateTB.UpdateUser = updater;
                                    context.FacilityItemValueTbs.Update(UpdateTB);
                                }
                                else
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return false;
                                }
                            }

                            // 삭제작업
                            foreach (int DelID in delIdx)
                            {
                                FacilityItemValueTb? DeleteTB = await context.FacilityItemValueTbs
                                    .FirstOrDefaultAsync(m => m.Id == DelID && m.DelYn != true)
                                    .ConfigureAwait(false);

                                if (DeleteTB is not null)
                                {
                                    DeleteTB.DelDt = ThisDate;
                                    DeleteTB.DelYn = true;
                                    DeleteTB.DelUser = updater;
                                    context.FacilityItemValueTbs.Update(DeleteTB);
                                }
                                else
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return false;
                                }
                            }
                        }
                        else // DELETE
                        {
                            if (ValueList is [_, ..])
                            {
                                foreach (FacilityItemValueTb ValueTB in ValueList)
                                {
                                    ValueTB.DelDt = ThisDate;
                                    ValueTB.DelYn = true;
                                    ValueTB.DelUser = updater;
                                    context.FacilityItemValueTbs.Update(ValueTB);
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
        /// Key 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteKeyInfo(FacilityItemKeyTb model)
        {
            try
            {
                context.FacilityItemKeyTbs.Update(model);
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
        /// 그룹 Key 리스트 삭제 - Value 까지 삭제됨
        /// </summary>
        /// <param name="KeyList"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteKeyList(List<int> KeyList, string deleter)
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
                        foreach (int KeyId in KeyList)
                        {
                            FacilityItemKeyTb? KeyTB = await context.FacilityItemKeyTbs
                            .FirstOrDefaultAsync(m => m.Id == KeyId && m.DelYn != true)
                            .ConfigureAwait(false);

                            if (KeyTB is null)
                                return (bool?)null;

                            KeyTB.DelDt = ThisDate;
                            KeyTB.DelUser = deleter;
                            KeyTB.DelYn = true;

                            context.FacilityItemKeyTbs.Update(KeyTB);

                            List<FacilityItemValueTb>? ValueList = await context.FacilityItemValueTbs
                            .Where(m => m.Id == KeyTB.Id && m.DelYn != true)
                            .ToListAsync()
                            .ConfigureAwait(false);

                            if (ValueList is [_, ..])
                            {
                                foreach (FacilityItemValueTb ValueTB in ValueList)
                                {
                                    ValueTB.DelDt = ThisDate;
                                    ValueTB.DelUser = deleter;
                                    ValueTB.DelYn = true;

                                    context.FacilityItemValueTbs.Update(ValueTB);
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
