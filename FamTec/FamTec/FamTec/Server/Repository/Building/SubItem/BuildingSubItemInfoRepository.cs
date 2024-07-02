using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building.SubItem
{
    public class BuildingSubItemInfoRepository : IBuildingSubItemInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingSubItemInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 건물 추가정보 등록
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<BuildingSubitemTb?> AddAsync(BuildingSubitemTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingSubitemTbs.Add(model);
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
        /// 건물ID에 해당하는 추가항목들 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteBuildingSubItemInfo(int? buildingid, string? username)
        {
            try
            {
                if (buildingid is not null && !String.IsNullOrWhiteSpace(username))
                {
                    List<BuildingSubitemTb>? model = await context.BuildingSubitemTbs.Where(m => m.BuildingTbId == buildingid).ToListAsync();

                    if (model is [_, ..])
                    {
                        for (int i = 0; i < model.Count(); i++)
                        {
                            model[i].DelYn = true;
                            model[i].DelDt = DateTime.Now;
                            model[i].DelUser = username;

                            context.BuildingSubitemTbs.Update(model[i]);
                        }

                        return await context.SaveChangesAsync() > 0 ? true : false;
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
        /// 건물ID에 등록되어있는 추가항목 데이터들 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingSubitemTb>?> GetAllBuildingSubItemList(int? buildingId)
        {
            try
            {
                if(buildingId is not null)
                {
                    List<BuildingSubitemTb>? model = await context.BuildingSubitemTbs.Where(m => m.BuildingTbId == buildingId && m.DelYn != true).ToListAsync();

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
        /// subid에 해당하는 모델 반환
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        public async ValueTask<BuildingSubitemTb?> GetBuildingSubItemInfo(int? subId)
        {
            try
            {
                if(subId is not null)
                {
                    BuildingSubitemTb? model = await context.BuildingSubitemTbs.FirstOrDefaultAsync(m => m.Id == subId);

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
        /// 건물 추가항목 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateBuildingSubItemInfo(BuildingSubitemTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingSubitemTbs.Update(model);
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
    }
}
