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
        public async ValueTask<DepartmentsTb?> AddAsync(DepartmentsTb model)
        {
            try
            {
                await context.DepartmentsTbs.AddAsync(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
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
                LogService.LogMessage(ex.Message);
                throw new ArgumentNullException();
            }
        }

      

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateDepartmentInfo(DepartmentsTb model)
        {
            try
            {
                context.DepartmentsTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
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
        public async ValueTask<List<DepartmentsTb>?> GetAllList()
        {
            try
            {
                List<DepartmentsTb>? model = await context.DepartmentsTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
        /// 관리부서 조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<DepartmentsTb>?> GetManageDepartmentList()
        {
            try
            {
                List<DepartmentsTb>? model = await context.DepartmentsTbs
                    .Where(m => m.DelYn != true && m.ManagementYn == true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
        public async ValueTask<DepartmentsTb?> GetDepartmentInfo(int Id)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(Id) && m.DelYn != true);

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
        /// 부서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentname"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<DepartmentsTb?> GetDepartmentInfo(string Name)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Name.Equals(Name) && m.DelYn != true);

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
        /// 선택한 부서에 관리자 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        public async ValueTask<List<AdminTb>?> SelectDepartmentAdminList(List<int> departmentidx)
        {
            try
            {
                List<AdminTb>? model = await context.AdminTbs
                    .Where(m => departmentidx.Contains(Convert.ToInt32(m.DepartmentTbId)) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
        /// 삭제할 부서 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<DepartmentsTb?> GetDeleteDepartmentInfo(int id)
        {
            try
            {
                DepartmentsTb? model = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

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
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteDepartment(DepartmentsTb model)
        {
            try
            {
                context.DepartmentsTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 부서 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteDepartmentInfo(List<int> idx, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int dpId in idx)
                    {
                        DepartmentsTb? DepartmentTB = await context.DepartmentsTbs
                            .FirstOrDefaultAsync(m => m.Id == dpId && m.DelYn != true);

                        if(DepartmentTB is not null)
                        {
                            DepartmentTB.DelYn = true;
                            DepartmentTB.DelUser = deleter;
                            DepartmentTB.DelDt = DateTime.Now;

                            context.DepartmentsTbs.Update(DepartmentTB);
                            bool DepartmentResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if(!DepartmentResult)
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                        else // 잘못됨
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

  
    }
}
