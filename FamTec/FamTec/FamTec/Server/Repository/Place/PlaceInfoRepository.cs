using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Place
{
    public class PlaceInfoRepository : IPlaceInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public PlaceInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<PlaceTb?> AddPlaceInfo(PlaceTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.PlaceTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
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
        /// 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<PlaceTb>?> GetAllList()
        {
            try
            {
                List<PlaceTb>? model = await context.PlaceTbs.Where(m => m.DelYn != true).ToListAsync();

                if (model is [_, ..])
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
        /// 사업장인덱스로 사업장 조회
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetByPlaceInfo(int? id)
        {
            try
            {
                if (id is not null)
                {
                    PlaceTb? model = await context.PlaceTbs
                        .FirstOrDefaultAsync(m => m.Id.Equals(id) &&
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
        /// 삭제할 사업장 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetDeletePlaceInfo(int? id)
        {
            try
            {
                if (id is not null)
                {
                    PlaceTb? model = await context.PlaceTbs
                        .FirstOrDefaultAsync(m => m.Id.Equals(id));

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

        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="placecd"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeletePlace(PlaceTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.PlaceTbs.Update(model);
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
        /// 삭제 테스트 해야함.
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> DeletePlaceList(string? Name, List<int>? placeidx)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(Name))
                        return null;
                    
                    if(placeidx is [_, ..])
                    {
                        foreach(int PlaceID in placeidx)
                        {
                            PlaceTb? PlaceTB = await context.PlaceTbs.FirstOrDefaultAsync(m => m.Id == PlaceID && m.DelYn != true);
                            if(PlaceTB is not null)
                            {
                                PlaceTB.DelYn = true;
                                PlaceTB.DelDt = DateTime.Now;
                                PlaceTB.DelUser = Name;

                                context.PlaceTbs.Update(PlaceTB);
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return false;
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
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> EditPlaceInfo(PlaceTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.PlaceTbs.Update(model);
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
