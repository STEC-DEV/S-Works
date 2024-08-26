using DocumentFormat.OpenXml.Spreadsheet;
using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.User
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public UserInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사용자 추가
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<UsersTb?> AddAsync(UsersTb model)
        {
            try
            {
                await context.UsersTbs.AddAsync(model);
             
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<bool?> AddUserAsync(UsersTb model)
        {
            try
            {
                UsersTb? search = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.UserId == model.UserId);

                if (search is null)
                {
                    context.UsersTbs.Add(model);
                    return await context.SaveChangesAsync() > 0 ? true: false;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="UserList"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddUserList(List<UsersTb> UserList)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    //foreach(UsersTb UserTB in UserList)
                    //{
                    //    context.UsersTbs.Add(UserTB);
                    //}
                    
                    await context.UsersTbs.AddRangeAsync(UserList);

                    bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(AddResult)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }


        /// <summary>
        /// 유저 INDEX로 유저테이블 조회
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async ValueTask<UsersTb?> GetUserIndexInfo(int useridx)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.Id == useridx && m.DelYn != true);
                    
                if(model is not null)
                    return model;
                else
                    return null;
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 유저 인덱스로 유저테이블 조회 (삭제유무와 상관없는 모든 사용자)
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async ValueTask<UsersTb?> GetNotFilterUserInfo(int useridx)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.Id == useridx);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        /// <summary>
        /// 유저테이블 모델 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteUserInfo(List<int> delIdx, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int UserId in delIdx)
                    {
                        UsersTb? UserTB = await context.UsersTbs.FirstOrDefaultAsync(m => m.Id == UserId && m.DelYn != true);
                        if(UserTB is not null)
                        {
                            UserTB.DelYn = true;
                            UserTB.DelDt = DateTime.Now;
                            UserTB.DelUser = deleter;
                            context.UsersTbs.Update(UserTB);
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(UpdateResult)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }


        /// <summary>
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<UsersTb?> GetUserInfo(string userid, string password)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => 
                    m.UserId!.Equals(userid) &&
                    m.Password!.Equals(password));

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<UsersTb?> UserIdCheck(string userid)
        {
            try
            {
                UsersTb? model = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.UserId == userid);
                
                if(model is not null)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 기계 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetVocMachineList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocMachine == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당사업장에 관리자가 아닌 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetPlaceUserList(int placeidx)
        {
            try
            {
                List<UsersTb>? model = await context.UsersTbs.Where(m => 
                m.PlaceTbId == placeidx &&
                m.AdminYn != true &&
                m.DelYn != true).ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetVocElecList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocElec == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 승강 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetVocLiftList(int placeidx)
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
                
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocLift == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 소방 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<UsersTb>?> GetVocFireList(int placeidx)
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
               
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocFire == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }

        }

        /// <summary>
        /// 해당 사업장의 건축 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<UsersTb>?> GetVocConstructList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocConstruct == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 통신 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetVocNetWorkList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocNetwork == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 미화 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<UsersTb>?> GetVocBeautyList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocBeauty == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
               
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 보안 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<UsersTb>?> GetVocSecurityList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocSecurity == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장의 기타 Voc권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetVocDefaultList(int placeidx)
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
                List<UsersTb>? model = await context.UsersTbs.Where(m =>
                m.PlaceTbId == placeidx &&
                m.PermVoc == 2 &&
                m.AlarmYn == true &&
                m.DelYn != true &&
                m.VocEtc == true).ToListAsync();

                if (model is not null)
                    return model;
                else
                    return null;
                
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사용자 데이터 삭제
        /// </summary>
        /// <param name="Useridx"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async ValueTask<int?> DeleteUserList(List<int> Useridx,string Name)
        {
            try
            {
                int count = 0;

                List<UsersTb>? UserList = context.UsersTbs.Where(m => Useridx.Contains(m.Id)).ToList();
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<UsersTb?> UpdateUserInfo(UsersTb model)
        {
            try
            {
                context.Update(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 테이블 전체 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<UsersTb>?> GetAllUserList()
        {
            try
            {
                List<UsersTb>? UserList = await context.UsersTbs
                    .Where(m => m.DelYn != true)
                    .ToListAsync();

                if (UserList is [_, ..])
                    return UserList;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

       
    }
}
