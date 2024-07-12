using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

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
                    List<BuildingTb>? model = await context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToListAsync();

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
                    BuildingTb? model = await context.BuildingTbs.FirstOrDefaultAsync(m => m.Id == buildingId && m.DelYn != true);

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
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateBuildingInfo(BuildingTb? model)
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

        /// <summary>
        /// 삭제할 리스트 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<BuildingTb>?> GetDeleteList(List<int>? buildingId)
        {
            try
            {
                
                if(buildingId is [_, ..])
                {
                    List<BuildingTb>? buildinglist = await context.BuildingTbs.Where(m => buildingId.Contains(m.Id) && m.DelYn != true).ToListAsync();

                    if (buildinglist is [_, ..])
                    {
                        // 층 Table의 DelYN = true
                        List<BuildingTb> BuildingTb = (from buildingtb in buildinglist
                                                       join floortb in context.FloorTbs.Where(m => m.DelYn == true)
                                                       on buildingtb.Id equals floortb.BuildingTbId
                                                       select buildingtb).ToList();
                        
                        // 건물 Table의 층이 할당안된 테이블
                        List<BuildingTb>? deleteList = buildinglist.Where(a => !context.FloorTbs.Any(b => b.BuildingTbId == a.Id)).ToList();
                        
                        // Merge
                        deleteList.AddRange(BuildingTb);

                        if (deleteList is [_, ..])
                            return deleteList;
                        else
                            return null;
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
        /// 제네릭에 해당하는 빌딩정보들 반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> GetBuildings(List<int>? buildingid)
        {
            try
            {
                if (buildingid is [_, ..])
                {
                    List<BuildingTb>? model = await context.BuildingTbs.Where(m => buildingid.Contains(m.Id)).ToListAsync();
                    if (model is [_, ..])
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<bool?> DeleteBuildingInfo(BuildingTb? model)
        {
            try
            {
                if (model is not null)
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

        /// <summary>
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> SelectPlaceBuildingList(List<int>? placeidx)
        {
            try
            {
                if(placeidx is [_, ..])
                {
                    List<BuildingTb>? buildingtb = await context.BuildingTbs.Where(m => placeidx.Contains(Convert.ToInt32(m.PlaceTbId)) && m.DelYn != true).ToListAsync();

                    if(buildingtb is [_, ..])
                    {
                        return buildingtb;
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

    }
}
