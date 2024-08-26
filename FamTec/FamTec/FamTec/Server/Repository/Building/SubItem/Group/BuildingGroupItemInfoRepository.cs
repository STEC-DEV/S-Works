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
        public async ValueTask<BuildingItemGroupTb?> AddAsync(BuildingItemGroupTb model)
        {
            try
            {
                
                await context.BuildingItemGroupTbs.AddAsync(model);
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
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹리스트 상세검색 buildingid로 검색
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemGroupTb>?> GetAllGroupList(int buildingid)
        {
            try
            {
                
                List<BuildingItemGroupTb>? model = await context.BuildingItemGroupTbs
                    .Where(m => m.BuildingTbId == buildingid && m.DelYn != true)
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
        /// 그룹 상세검색 groupid로 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemGroupTb?> GetGroupInfo(int groupid)
        {
            try
            {
                BuildingItemGroupTb? model = await context.BuildingItemGroupTbs
                    .FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true);

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
        /// 그룹수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateGroupInfo(BuildingItemGroupTb model)
        {
            try
            {
                context.BuildingItemGroupTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(BuildingItemGroupTb model)
        {
            try
            {
                context.BuildingItemGroupTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹삭제 - 2 Transaction
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(int groupid, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    BuildingItemGroupTb? GroupTB = await context.BuildingItemGroupTbs
                        .FirstOrDefaultAsync(m => m.Id == groupid);

                    if(GroupTB is not null)
                    {
                        GroupTB.DelDt = DateTime.Now;
                        GroupTB.DelUser = deleter;
                        GroupTB.DelYn = true;

                        context.BuildingItemGroupTbs.Update(GroupTB);

                        List<BuildingItemKeyTb>? KeyTB = await context.BuildingItemKeyTbs.Where(m => m.BuildingGroupTbId == groupid).ToListAsync();
                        if(KeyTB is [_, ..])
                        {
                            foreach(BuildingItemKeyTb KeyModel in KeyTB)
                            {
                                KeyModel.DelDt = DateTime.Now;
                                KeyModel.DelUser = deleter;
                                KeyModel.DelYn = true;

                                context.BuildingItemKeyTbs.Update(KeyModel);

                                List<BuildingItemValueTb>? ValueTB = await context.BuildingItemValueTbs.Where(m => m.BuildingKeyTbId == KeyModel.Id).ToListAsync();
                                if(ValueTB is [_, ..])
                                {
                                    foreach(BuildingItemValueTb ValueModel in ValueTB)
                                    {
                                        ValueModel.DelDt = DateTime.Now;
                                        ValueModel.DelUser = deleter;
                                        ValueModel.DelYn = true;
                                        
                                        context.BuildingItemValueTbs.Update(ValueModel);
                                    }
                                }
                            }
                        }

                        bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(DeleteResult)
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
        }

        /// <summary>
        /// 넘어온 Id에 포함되어있는 GroupTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemGroupTb>?> ContainsGroupList(List<int> GroupId, int buildingid)
        {
            try
            {
                List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs
                    .Where(e => GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
        public async ValueTask<List<BuildingItemGroupTb>?> NotContainsGroupList(List<int> GroupId, int buildingid)
        {
            try
            {
                List<BuildingItemGroupTb>? grouptb = await context.BuildingItemGroupTbs
                    .Where(e => !GroupId.Contains(e.Id) && e.Id == buildingid && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
