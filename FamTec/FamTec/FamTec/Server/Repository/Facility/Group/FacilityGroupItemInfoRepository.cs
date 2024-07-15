using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Facility.Group
{
    public class FacilityGroupItemInfoRepository : IFacilityGroupItemInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FacilityGroupItemInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemGroupTb?> AddAsync(FacilityItemGroupTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FacilityItemGroupTbs.Add(model);
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
        /// 설비ID에 포함되어있는 그룹List 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityItemGroupTb>?> GetAllGroupList(int? facilityId)
        {
            try
            {
                if (facilityId is not null)
                {
                    List<FacilityItemGroupTb>? model = await context.FacilityItemGroupTbs.Where(m => m.FacilityTbId == facilityId && m.DelYn != true).ToListAsync();
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
        /// 그룹ID로 모델 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemGroupTb?> GetGroupInfo(int? groupid)
        {
            try
            {
                if(groupid is not null)
                {
                    FacilityItemGroupTb? model = await context.FacilityItemGroupTbs.FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true);
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
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateGroupInfo(FacilityItemGroupTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FacilityItemGroupTbs.Update(model);
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
        /// 그룹사겢
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(FacilityItemGroupTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.FacilityItemGroupTbs.Update(model);
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
