using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.User
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<UserInfoRepository> CreateBuilderLogger;

        public UserInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<UserInfoRepository> _createbuilderlogger)
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool?> DelUserCheck(int id)
        {
            try
            {
                bool AdminCheck = await context.AdminTbs.AnyAsync(m => m.UserTbId == id && m.DelYn != true).ConfigureAwait(false);
                //bool AlarmCheck = await context.AlarmTbs.AnyAsync(m => m.UsersTbId == id && m.DelYn != true).ConfigureAwait(false);
                //bool CommentCheck = await context.CommentTbs.AnyAsync(m => m.UserTbId == id && m.DelYn != true).ConfigureAwait(false);

                return AdminCheck;
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
        /// 사용자 추가
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<UsersTb?> AddAsync(UsersTb model)
        {
            try
            {
                await context.UsersTbs.AddAsync(model).ConfigureAwait(false);
             
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

        public async Task<bool?> AddUserAsync(UsersTb model)
        {
            try
            {
                UsersTb? search = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.UserId == model.UserId)
                    .ConfigureAwait(false);

                if(search is not null)
                    return false;
                else
                    context.UsersTbs.Add(model);
                    return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true: false;
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
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="UserList"></param>
        /// <returns></returns>
        public async Task<bool?> AddUserList(List<UsersTb> UserList)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        await context.UsersTbs.AddRangeAsync(UserList).ConfigureAwait(false);

                        bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (AddResult)
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
        /// 유저 INDEX로 유저테이블 조회
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async Task<UsersTb?> GetUserIndexInfo(int useridx)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.Id == useridx && m.DelYn != true)
                    .ConfigureAwait(false);
                    
                if(model is not null)
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
        /// 유저 인덱스로 유저테이블 조회 (삭제유무와 상관없는 모든 사용자)
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async Task<UsersTb?> GetNotFilterUserInfo(int useridx)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.Id == useridx)
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
        /// 유저테이블 모델 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteUserInfo(List<int> delIdx, string deleter)
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
                        foreach (int UserId in delIdx)
                        {
                            UsersTb? UserTB = await context.UsersTbs
                            .FirstOrDefaultAsync(m => m.Id == UserId && m.DelYn != true)
                            .ConfigureAwait(false);

                            if (UserTB is not null)
                            {
                                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                UserTB.UserId = $"{UserTB.UserId}_{UserTB.Id}";
                                UserTB.DelYn = true;
                                UserTB.DelDt = ThisDate;
                                UserTB.DelUser = deleter;
                                context.UsersTbs.Update(UserTB);
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }

                        bool UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (UpdateResult)
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
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<UsersTb?> GetUserInfo(string userid, string password)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => 
                    m.UserId!.Equals(userid) &&
                    m.Password!.Equals(password))
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
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<UsersTb?> UserIdCheck(string userid)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.UserId == userid && m.DelYn != true)
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
        /// 해당 사업장의 기계 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetVocMachineList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 기계인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocMachine == true)
                    .ToListAsync()
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
        /// 해당사업장에 관리자가 아닌 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetPlaceUserList(int placeidx)
        {
            try
            {
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.AdminYn != true &&
                                m.DelYn != true)
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
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetVocElecList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 전기 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocElec == true)
                    .ToListAsync()
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
        /// 해당 사업장의 승강 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetVocLiftList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 승강인 List 반환
                 */
                
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocLift == true)
                    .ToListAsync()
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
        /// 해당 사업장의 소방 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<UsersTb>?> GetVocFireList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 소방인 List 반환
                 */
               
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocFire == true)
                    .ToListAsync()
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
        /// 해당 사업장의 건축 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<UsersTb>?> GetVocConstructList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 건축인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocConstruct == true)
                    .ToListAsync()
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
        /// 해당 사업장의 통신 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetVocNetWorkList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 통신인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocNetwork == true)
                    .ToListAsync()
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
        /// 해당 사업장의 미화 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<UsersTb>?> GetVocBeautyList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 미화인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocBeauty == true)
                    .ToListAsync()
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
        /// 해당 사업장의 보안 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<UsersTb>?> GetVocSecurityList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 보안인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocSecurity == true)
                    .ToListAsync()
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
        /// 해당 사업장의 기타 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetVocDefaultList(int placeidx)
        {
            try
            {
                /* 
                 * 유저 정보의 사업장이 넘어온 사업장과 같고 && 
                 * 민원관리 권한이 수정-보기 권한이고 &&
                 * 삭제된 사용자가 아니고
                 * 알람 받기유무가 Yes 이고
                 * Voc권한이 기계인 List 반환
                 */
                List<UsersTb>? model = await context.UsersTbs
                    .Where(m => m.PlaceTbId == placeidx &&
                                m.PermVoc == 2 &&
                                m.AlarmYn == true &&
                                m.DelYn != true &&
                                m.VocEtc == true)
                    .ToListAsync()
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
        /// 사용자 데이터 삭제
        /// </summary>
        /// <param name="Useridx"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<int?> DeleteUserList(List<int> Useridx,string Name)
        {
            try
            {
                int count = 0;

                List<UsersTb>? UserList = await context.UsersTbs
                    .Where(m => Useridx.Contains(m.Id))
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach(var Users in UserList)
                {
                    Users.DelYn = true;
                    Users.DelDt = DateTime.Now;
                    Users.DelUser = Name;

                    context.Update(Users);
                    count++;
                }
                    
                await context.SaveChangesAsync();
                return count;
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
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UsersTb?> UpdateUserInfo(UsersTb model)
        {
            try
            {
                context.Update(model);
                await context.SaveChangesAsync().ConfigureAwait(false);
                return model;
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
        /// 테이블 전체 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsersTb>?> GetAllUserList()
        {
            try
            {
                List<UsersTb>? UserList = await context.UsersTbs
                    .Where(m => m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (UserList is [_, ..])
                    return UserList;
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
