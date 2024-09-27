using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Unit
{
    public class UnitInfoRepository : IUnitInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public UnitInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UnitTb?> AddAsync(UnitTb model)
        {
            try
            {
                await context.UnitTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 사업장별 단위 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<UnitTb>?> GetUnitList(int placeid)
        {
            try
            {
                List<UnitTb>? model = await context.UnitTbs
                    .Where(m => m.PlaceTbId == null || m.PlaceTbId == placeid && m.DelYn != true)
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
        /// 단위정보 인덱스로 단위모델 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        public async Task<UnitTb?> GetUnitInfo(int UnitIdx)
        {
            try
            {
                UnitTb? model = await context.UnitTbs
                    .FirstOrDefaultAsync(m => m.Id == UnitIdx && m.DelYn != true)
                    .ConfigureAwait(false);

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
        /// 해당사업장에 단위 추가되는지 여부
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<bool?> AddUnitInfoCheck(string unit, int placeid)
        {
            try
            {
                UnitTb? UnitInfo = await context.UnitTbs
                    .FirstOrDefaultAsync(m => m.Unit == unit && 
                                              m.PlaceTbId == placeid && 
                                              m.DelYn != true)
                    .ConfigureAwait(false);

                if (UnitInfo is not null)
                    return false;
                else
                    return true;
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
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteUnitInfo(List<int> idx, string deleter)
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
                        foreach (int unitid in idx)
                        {
                            UnitTb? UnitModel = await context.UnitTbs
                                .FirstOrDefaultAsync(m => m.Id == unitid &&
                                                            m.DelYn != true)
                                .ConfigureAwait(false);

                            if (UnitModel is null)
                                return (bool?)null;

                            // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                            UnitModel.Unit = $"{UnitModel.Unit}_{UnitModel.Id}";
                            UnitModel.DelDt = ThisDate;
                            UnitModel.DelUser = deleter;
                            UnitModel.DelYn = true;

                            context.UnitTbs.Update(UnitModel);
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
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateUnitInfo(UnitTb model)
        {
            try
            {
                context.UnitTbs.Update(model);
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
