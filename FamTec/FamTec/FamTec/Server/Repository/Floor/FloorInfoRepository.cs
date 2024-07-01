using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FamTec.Server.Repository.Floor
{
    public class FloorInfoRepository : IFloorInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FloorInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 층추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FloorTb?> AddAsync(FloorTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FloorTbs.Add(model);
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
        /// 층삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteFloorInfo(FloorTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FloorTbs.Update(model);
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
        /// 층 인덱스로 층 검색
        /// </summary>
        /// <param name="flooridx"></param>
        /// <returns></returns>
        public async ValueTask<FloorTb?> GetFloorInfo(int? flooridx)
        {
            try
            {
                if(flooridx is not null)
                {
                    FloorTb? model = await context.FloorTbs.FirstOrDefaultAsync(m => m.Id == flooridx && m.DelYn != true);

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
        /// 건물에 해당하는 층 리스트 조회
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async ValueTask<List<FloorTb>?> GetFloorList(int? buildingtbid)
        {
            try
            {
                if(buildingtbid is not null)
                {
                    List<FloorTb>? model = await context.FloorTbs.Where(m => m.BuildingTbId == buildingtbid && m.DelYn != true).ToListAsync();

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
        /// 건물에 해당하는 층List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<FloorTb>?> GetFloorList(List<BuildingTb?> model)
        {
            try
            {
                if(model is [_, ..])
                {
                    List<FloorTb>? floor = await context.FloorTbs.Where(m => m.DelYn != true).ToListAsync();

                    if (floor is [_, ..])
                    {
                        List<FloorTb>? result = (from bdtb in model
                                                 join floortb in floor
                                                 on bdtb.Id equals floortb.BuildingTbId
                                                 where floortb.DelYn != true && bdtb.DelYn != true
                                                 select new FloorTb
                                                 {
                                                     Id = floortb.Id,
                                                     Name = floortb.Name,
                                                     CreateDt = floortb.CreateDt,
                                                     CreateUser = floortb.CreateUser,
                                                     UpdateDt = floortb.UpdateDt,
                                                     UpdateUser = floortb.UpdateUser,
                                                     DelYn = floortb.DelYn,
                                                     DelDt = floortb.DelDt,
                                                     DelUser = floortb.DelUser,
                                                     BuildingTbId = floortb.BuildingTbId
                                                 }).ToList();

                        if (result is [_, ..])
                            return result;
                        else
                            return null;
                    }
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


    }
}
