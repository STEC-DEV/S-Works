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
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteDepartmentInfo(List<int?> selList)
        {
            try
            {
                if(selList is [_, ..])
                {
                    for (int i = 0; i < selList.Count; i++)
                    {
                        DepartmentTb? departmenttb = await context.DepartmentTbs.FirstOrDefaultAsync(m => m.Id == selList[i] && m.DelYn != true);
                        
                        if (departmenttb is not null)
                        {
                            AdminTb? admintb = await context.AdminTbs.FirstOrDefaultAsync(m => m.DepartmentTbId == departmenttb.Id && m.DelYn != true);
                            
                            if(admintb is null)
                            {
                                departmenttb.DelYn = true;
                                departmenttb.DelDt = DateTime.Now;
                                
                                context.DepartmentTbs.Update(departmenttb);
                            }
                            else
                            {
                                // 할당된 사용자가 있음
                                return false;
                            }
                        }
                        else
                        {
                            // 부서가 없음
                            return null;
                        }
                    }
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    // 조건이 잘못됨
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

     
    }
}
