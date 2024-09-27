using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.BlackList
{
    public class BlackListInfoRepository : IBlackListInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BlackListInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public async Task<BlacklistTb?> AddAsync(BlacklistTb model)
        {
            try
            {
                await context.BlacklistTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 블랙리스트 전체조회
        /// </summary>
        /// <returns></returns>
        public async Task<List<BlacklistTb>?> GetBlackList()
        {
            try
            {
                List<BlacklistTb>? model = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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
        /// 블랙리스트 페이지네이션 조회
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<BlacklistTb>?> GetBlackListPaceNationList(int pagenumber, int pagesize)
        {
            try
            {
                List<BlacklistTb>? model = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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
        /// 블랙리스트 개수 반환
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetBlackListCount()
        {
            try
            {
                int count = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
                    .CountAsync()
                    .ConfigureAwait(false);

                return count;
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
        /// 블랙리스트 전화번호로 조회
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public async Task<BlacklistTb?> GetBlackListInfo(string PhoneNumber)
        {
            try
            {
                BlacklistTb? model = await context.BlacklistTbs
                    .FirstOrDefaultAsync(m => m.Phone == PhoneNumber && m.DelYn != true)
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
        /// 블랙리스트 ID로 추가
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlacklistTb?> GetBlackListInfo(int id)
        {
            try
            {
                BlacklistTb? model = await context.BlacklistTbs
                    .FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true)
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
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteBlackList(List<int> delIdx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        foreach (int BlackListID in delIdx)
                        {
                            BlacklistTb? BlackListTB = await context.BlacklistTbs
                                .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == BlackListID)
                                .ConfigureAwait(false);

                            if (BlackListTB is not null)
                            {
                                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                BlackListTB.Phone = $"{BlackListTB.Phone}_{BlackListTB.Id}";
                                BlackListTB.DelYn = true;
                                BlackListTB.DelDt = ThisDate;
                                BlackListTB.DelUser = deleter;

                                context.BlacklistTbs.Update(BlackListTB);
                            }
                            else
                            {
                                // 조회결과가 없음
                                return (bool?)null;
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
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateBlackList(BlacklistTb model)
        {
            try
            {
                context.BlacklistTbs.Update(model);
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
