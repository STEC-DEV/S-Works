using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

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
        public async ValueTask<UserTb?> AddAsync(UserTb? model)
        {
            try
            {
                if (model is not null)
                {
                    UserTb? search = await context.UserTbs.FirstOrDefaultAsync(m => m.UserId == model.UserId);
                    if (search is null)
                    {
                        context.UserTbs.Add(model);
                        await context.SaveChangesAsync();
                        return model;
                    }
                    else
                    {
                        return null;
                    }
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
        /// 유저 INDEX로 유저테이블 조회
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async ValueTask<UserTb?> GetUserIndexInfo(int? useridx)
        {
            try
            {
                if(useridx is not null)
                {
                    UserTb? model = await context.UserTbs.FirstOrDefaultAsync(m => m.Id == useridx && m.DelYn != true);
                    
                    if(model is not null)
                        return model;
                    else
                        return null;
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
        /// 유저테이블 모델 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteUserInfo(UserTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.UserTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<UserTb?> GetUserInfo(string? userid, string? password)
        {
            try
            {
                if(!String.IsNullOrWhiteSpace(userid) && !String.IsNullOrWhiteSpace(password))
                {
                    UserTb? model = await context.UserTbs
                        .FirstOrDefaultAsync(m => 
                        m.UserId!.Equals(userid) &&
                        m.Password!.Equals(password));

                    if (model is not null)
                        return model;
                    else
                        return null;
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
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<UserTb?> UserIdCheck(string? userid)
        {
            try
            {
                if(!String.IsNullOrWhiteSpace(userid))
                {
                    UserTb? model = await context.UserTbs.FirstOrDefaultAsync(m => m.UserId == userid);
                    if(model is not null)
                    {
                        return model;
                    }
                    else
                    {
                        return null;
                    }
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
        public async ValueTask<List<UserTb>?> GetVocMachineList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocMachine == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
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
        public async ValueTask<List<UserTb>?> GetPlaceUserList(int? placeidx)
        {
            try
            {
                if(placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m => 
                    m.PlaceTbId == placeidx &&
                    m.AdminYn != 1 &&
                    m.DelYn != true).ToListAsync();

                    if (model is [_, ..])
                        return model;
                    else
                        return null;
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
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<UserTb>?> GetVocElecList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocElec == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocLiftList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocLift == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocFireList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocFire == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocConstructList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocConstruct == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocNetWorkList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocNetwork == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocBeautyList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocBeauty == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocSecurityList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocSecurity == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<List<UserTb>?> GetVocDefaultList(int? placeidx)
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
                if (placeidx is not null)
                {
                    List<UserTb>? model = await context.UserTbs.Where(m =>
                    m.PlaceTbId == placeidx &&
                    m.PermVoc == 2 &&
                    m.AlramYn == 1 &&
                    m.DelYn != true &&
                    m.VocDefault == 1).ToListAsync();

                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
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
        public async ValueTask<int?> DeleteUserList(List<int>? Useridx,string? Name)
        {
            try
            {
                int count = 0;

                if (Useridx is [_, ..])
                {
                    List<UserTb>? UserList = context.UserTbs.Where(m => Useridx.Contains(m.Id)).ToList();
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
                else
                {
                    return count;
                }
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
        public async ValueTask<UserTb?> UpdateUserInfo(UserTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.Update(model);
                    await context.SaveChangesAsync();
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
        /// 테이블 전체 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<UserTb>?> GetAllUserList()
        {
            try
            {
                List<UserTb>? UserList = await context.UserTbs.Where(m => m.DelYn != true).ToListAsync();
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
