using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin.Place;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Admin.AdminUser
{
    public class AdminUserInfoRepository : IAdminUserInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public AdminUserInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> AddAdminUserInfo(AdminTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.AdminTbs.Add(model);
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
        /// 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> DeleteAdminInfo(AdminTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.AdminTbs.Update(model);
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
        /// 매개변수의 관리자ID에 해당하는 관리자모델 모델 조회
        /// </summary>
        /// <param name="adminuseridx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<AdminTb?> GetAdminUserInfo(int? usertbid)
        {
            try
            {
                if (usertbid is not null)
                {
                    AdminTb? model = await context.AdminTbs.FirstOrDefaultAsync(m => m.UserTbId.Equals(usertbid) && m.DelYn != true);
                    if (model is not null)
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



        /// <summary>
        /// 관리자 DTO 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<ManagerListDTO>?> GetAllAdminUserList()
        {
            try
            {
                List<ManagerListDTO>? model = await context.AdminTbs
                    .Where(m => m.DelYn != true)
                    .Include(m => m.UserTb)
                    .Include(m => m.DepartmentTb)
                    .Select(m => new ManagerListDTO
                {
                    Id = m.Id,
                    UserId = m.UserTb!.UserId!,
                    Name = m.UserTb.Name!,
                    Department = m.DepartmentTb!.Name!
                }).ToListAsync();
                
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

 
    }
}
