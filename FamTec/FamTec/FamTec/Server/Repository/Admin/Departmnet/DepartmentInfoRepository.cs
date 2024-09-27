using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Admin.Departmnet
{
    public class DepartmentInfoRepository : IDepartmentInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public DepartmentInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        public async Task<bool?> DelDepartmentCheck(int departmentid)
        {
            try
            {
                bool AdminCheck = await context.AdminTbs.AnyAsync(m => m.DepartmentTbId == departmentid && m.DelYn != true).ConfigureAwait(false);
                bool DepartmentCheck = await context.PlaceTbs.AnyAsync(m => m.DepartmentTbId == departmentid && m.DelYn != true).ConfigureAwait(false);

                return AdminCheck || DepartmentCheck;
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
        /// 부서추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DepartmentsTb?> AddAsync(DepartmentsTb model)
        {
            try
            {
                await context.DepartmentsTbs.AddAsync(model).ConfigureAwait(false);

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
                LogService.LogMessage(ex.Message);
                throw;
            }
        }

      

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateDepartmentInfo(DepartmentsTb model)
        {
            try
            {
                context.DepartmentsTbs.Update(model);
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
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<DepartmentsTb>?> GetAllList()
        {
            try
            {
                List<DepartmentsTb>? model = await context.DepartmentsTbs
                    .Where(m => m.DelYn != true)
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
        /// 관리부서 조회
        /// </summary>
        /// <returns></returns>
        public async Task<List<DepartmentsTb>?> GetManageDepartmentList()
        {
            try
            {
                List<DepartmentsTb>? model = await context.DepartmentsTbs
                    .Where(m => m.DelYn != true && m.ManagementYn == true)
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
        /// 부서IDX에 해당하는 단일 모델 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        public async Task<DepartmentsTb?> GetDepartmentInfo(int Id)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(Id) && m.DelYn != true)
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
        /// 부서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentname"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<DepartmentsTb?> GetDepartmentInfo(string Name)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Name.Equals(Name) && m.DelYn != true)
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
        /// 선택한 부서에 관리자 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        public async Task<List<AdminTb>?> SelectDepartmentAdminList(List<int> departmentidx)
        {
            try
            {
                List<AdminTb>? model = await context.AdminTbs
                    .Where(m => departmentidx.Contains(Convert.ToInt32(m.DepartmentTbId)) && m.DelYn != true)
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
        /// 삭제할 부서 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<DepartmentsTb?> GetDeleteDepartmentInfo(int id)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(id))
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
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteDepartment(DepartmentsTb model)
        {
            try
            {
                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                model.Name = $"{model.Name}_{model.Id}"; 
                context.DepartmentsTbs.Update(model);
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
        /// 부서 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteDepartmentInfo(List<int> idx, string deleter)
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
                        foreach (int dpId in idx)
                        {
                            DepartmentsTb? DepartmentTB = await context.DepartmentsTbs
                                .FirstOrDefaultAsync(m => m.Id == dpId && m.DelYn != true)
                                .ConfigureAwait(false);

                            if (DepartmentTB is not null)
                            {
                                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                DepartmentTB.Name = $"{DepartmentTB.Name}_{DepartmentTB.Id}";
                                DepartmentTB.DelYn = true;
                                DepartmentTB.DelUser = deleter;
                                DepartmentTB.DelDt = ThisDate;

                                context.DepartmentsTbs.Update(DepartmentTB);
                            }
                            else // 잘못됨
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                        }

                        bool DepartmentResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (DepartmentResult)
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
                        return false;
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
