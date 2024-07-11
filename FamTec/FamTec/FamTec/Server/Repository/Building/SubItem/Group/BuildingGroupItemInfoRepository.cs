using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building.SubItem.Group
{
    public class BuildingGroupItemInfoRepository : IBuildingGroupItemInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingGroupItemInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<BuildingGroupitemTb?> AddAsync(BuildingGroupitemTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingGroupitemTbs.Add(model);
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
        /// 그룹리스트 상세검색 buildingid로 검색
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingGroupitemTb>?> GetAllGroupList(int? buildingid)
        {
            try
            {
                if(buildingid is not null)
                {
                    List<BuildingGroupitemTb>? model = await context.BuildingGroupitemTbs.Where(m => m.BuildingId == buildingid && m.DelYn != true).ToListAsync();

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
        /// 그룹 상세검색 groupid로 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingGroupitemTb?> GetGroupInfo(int? groupid)
        {
            try
            {
                if (groupid is not null)
                {
                    BuildingGroupitemTb? model = await context.BuildingGroupitemTbs.FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true);

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
        /// 그룹수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateGroupInfo(BuildingGroupitemTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingGroupitemTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(BuildingGroupitemTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.BuildingGroupitemTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 넘어온 Id에 포함되어있는 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingGroupitemTb>?> ContainsGroupList(List<int>? GroupId, int buildingid)
        {
            try
            {
                List<BuildingGroupitemTb>? grouptb = await context.BuildingGroupitemTbs.Where(e => GroupId.Contains(e.Id) && e.BuildingId == buildingid && e.DelYn != true).ToListAsync();
                if (grouptb is [_, ..])
                    return grouptb;
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
        /// 넘어온 Id에 포함되어있지 않은 GroupTb 반환
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingGroupitemTb>?> NotContainsGroupList(List<int>? GroupId, int buildingid)
        {
            try
            {
                List<BuildingGroupitemTb>? grouptb = await context.BuildingGroupitemTbs
                    .Where(e => !GroupId.Contains(e.Id) && e.BuildingId == buildingid && e.DelYn != true).ToListAsync();
                if (grouptb is [_, ..])
                    return grouptb;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
