using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

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
        /// 부서추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<DepartmentTb?> AddAsync(DepartmentTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.DepartmentTbs.Add(model);
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
                LogService.LogMessage(ex.Message);
                throw new ArgumentNullException();
            }
        }

      

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateDepartmentInfo(DepartmentTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.DepartmentTbs.Update(model);
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
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<List<DepartmentTb>?> GetAllList()
        {
            try
            {
                List<DepartmentTb>? model = await context.DepartmentTbs.Where(m => m.DelYn != true).ToListAsync();

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
        /// 부서IDX에 해당하는 단일 모델 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        public async ValueTask<DepartmentTb?> GetDepartmentInfo(int? Id)
        {
            try
            {
                if (Id is not null)
                {

                    DepartmentTb? model = await context.DepartmentTbs
                        .FirstOrDefaultAsync(m => m.Id.Equals(Id) &&
                        m.DelYn != true);

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
        /// 부서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentname"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<DepartmentTb?> GetDepartmentInfo(string? Name)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Name))
                {
                    DepartmentTb? model = await context.DepartmentTbs
                        .FirstOrDefaultAsync(m => m.Name.Equals(Name)
                        && m.DelYn != true);

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
        /// 선택한 부서에 관리자 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        public async ValueTask<List<AdminTb>?> SelectDepartmentAdminList(List<int>? departmentidx)
        {
            try
            {
                if(departmentidx is [_, ..])
                {
                    List<AdminTb>? model = await context.AdminTbs.Where(m => departmentidx.Contains(Convert.ToInt32(m.DepartmentTbId)) && m.DelYn != true).ToListAsync();

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
        /// 삭제할 부서 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<DepartmentTb?> GetDeleteDepartmentInfo(int? id)
        {
            try
            {
                if(id is not null)
                {
                    DepartmentTb? model = await context.DepartmentTbs.FirstOrDefaultAsync(m => m.Id.Equals(id));

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
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteDepartment(DepartmentTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.DepartmentTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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
