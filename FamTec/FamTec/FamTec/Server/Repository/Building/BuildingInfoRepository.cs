using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Abstractions;
using System.Reflection;

namespace FamTec.Server.Repository.Building
{
    public class BuildingInfoRepository : IBuildingInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<BuildingTb?> AddAsync(BuildingTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingTbs.Add(model);
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
        /// 해당사업장 건물 전체조회
        /// </summary>
        /// <param name="placeid">로그인한 사업장정보</param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> GetAllBuildingList(int? placeid)
        {
            try
            {
                if(placeid is not null)
                {
                    List<BuildingTb>? model = await context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != 1).ToListAsync();

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
        /// 빌딩 인덱스로 빌딩 검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async ValueTask<BuildingTb?> GetBuildingInfo(int? buildingId)
        {
            try
            {
                if(buildingId is not null)
                {
                    BuildingTb? model = await context.BuildingTbs.FirstOrDefaultAsync(m => m.Id == buildingId && m.DelYn != 1);

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
        /// 해당사업장 건물 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<bool?> DeleteBuildingInfo(BuildingTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingTbs.Update(model);
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
