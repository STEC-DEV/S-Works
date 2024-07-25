using FamTec.Server.Databases;
using FamTec.Server.Repository.Place;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Type.Electronic;
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
        /// 매개변수의 ADMINID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> GetAdminIdInfo(int? adminid)
        {
            if (adminid is not null)
            {
                AdminTb? model = await context.AdminTbs.FirstOrDefaultAsync(m => m.Id.Equals(adminid) && m.DelYn != true);
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



        /// <summary>
        /// 매개변수의 USERID에 해당하는 관리자모델 모델 조회
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

        /// <summary>
        /// 해당 사업장 관리자로 선택가능한 사업장 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<ManagerListDTO?>?> GetNotContainsAdminList(int? placeid)
        {
            try
            {
                if (placeid is not null)
                {
                    List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToListAsync();

                    if (adminplacetb is [_, ..])
                    {
                        List<int?> admintbid = adminplacetb.Select(m => m.AdminTbId).ToList();

                        List<AdminTb>? admintb = await context.AdminTbs.Where(e => !admintbid.Contains(e.Id) && e.DelYn != true).ToListAsync();
                        if (admintb is [_, ..])
                        {
                            List<ManagerListDTO>? model = (from Admin in admintb
                                                           join User in context.UsersTbs.Where(m => m.DelYn != true).ToList()
                                                           on Admin.UserTbId equals User.Id
                                                           join Department in context.DepartmentsTbs.Where(m => m.DelYn != true).ToList()
                                                           on Admin.DepartmentTbId equals Department.Id
                                                           select new ManagerListDTO()
                                                           {
                                                               Id = Admin.Id,
                                                               Name = User.Name,
                                                               UserId = User.UserId,
                                                               Department = Department.Name
                                                           }).ToList();

                            return model;
                        }
                        else
                        {
                            return null; // 전체 관리자 다 이사업장에 포함됨.
                        }
                    }
                    else
                    {
                        // 이사업장에 아무도 포함되어있지 않음
                        //List<AdminTb>? alladmin = await context.AdminTbs.Where(m => m.DelYn != true).ToListAsync();

                        List<ManagerListDTO>? model = (from Admin in context.AdminTbs.Where(m => m.DelYn != true).ToList()
                                                       join User in context.UsersTbs.Where(m => m.DelYn != true).ToList()
                                                       on Admin.UserTbId equals User.Id
                                                       join Department in context.DepartmentsTbs.Where(m => m.DelYn != true).ToList()
                                                       on Admin.DepartmentTbId equals Department.Id
                                                       select new ManagerListDTO()
                                                       {
                                                           Id = Admin.Id,
                                                           Name = User.Name,
                                                           UserId = User.UserId,
                                                           Department = Department.Name
                                                       }).ToList();


                        return model;
                        //return alladmin;
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
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> UpdateAdminInfo(AdminTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.AdminTbs.Update(model);
                    await context.SaveChangesAsync();
                    return model;
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

    }
}
