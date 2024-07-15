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
        public async ValueTask<BuildingItemGroupTb?> AddAsync(BuildingItemGroupTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingItemGroupTbs.Add(model);
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
        public async ValueTask<List<BuildingItemGroupTb>?> GetAllGroupList(int? buildingid)
        {
            try
            {
                if(buildingid is not null)
                {
                    List<BuildingItemGroupTb>? model = await context.BuildingItemGroupTbs.Where(m => m.BuildingTbId == buildingid && m.DelYn != true).ToListAsync();

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
        public async ValueTask<BuildingItemGroupTb?> GetGroupInfo(int? groupid)
        {
            try
            {
                if (groupid is not null)
                {
                    BuildingItemGroupTb? model = await context.BuildingItemGroupTbs.FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true);

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
        public async ValueTask<bool?> UpdateGroupInfo(BuildingItemGroupTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingItemGroupTbs.Update(model);
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
        public async ValueTask<bool?> DeleteGroupInfo(BuildingItemGroupTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.BuildingItemGroupTbs.Update(model);
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
        public async ValueTask<List<BuildingItemGroupTb>?> ContainsGroupList(List<int>? GroupId, int buildingid)
        {
            try
            {
                if (GroupId is [_, ..])
                {
                    List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs.Where(e => GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true).ToListAsync();
                    if (grouptb is [_, ..])
                        return grouptb;
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
        /// 넘어온 Id에 포함되어있지 않은 GroupTb 반환
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemGroupTb>?> NotContainsGroupList(List<int>? GroupId, int buildingid)
        {
            try
            {
                if (GroupId is [_, ..])
                {
                    List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs
                        .Where(e => !GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true).ToListAsync();
                    if (grouptb is [_, ..])
                        return grouptb;
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
