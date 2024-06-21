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
                List<PlaceTb>? model = await context.PlaceTbs.Where(m => m.DelYn == 0).ToListAsync();

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
        /// 사업장코드로 사업장 조회
        /// </summary>
        /// <param name="placecd"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetByPlaceInfo(string? placecd)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(placecd))
                {
                    PlaceTb? model = await context.PlaceTbs
                        .FirstOrDefaultAsync(m => m.PlaceCd.Equals(placecd) 
                        && m.DelYn != 1);

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
                        m.DelYn != 1);

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
        /// 삭제
        /// </summary>
        /// <param name="placecd"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeletePlaceInfo(PlaceTb? model)
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
            try
            {
                if (placeidx is [_, ..])
                {
                    //List<AdminPlaceTb>? adminplace = (from adminplacetb in context.AdminPlaceTbs.ToList()
                    //                                  join placetb in placeidx
                    //                                  on adminplacetb.PlaceId equals placetb
                    //                                  where adminplacetb.DelYn != 1
                    //                                  select new AdminPlaceTb()
                    //                                  {
                    //                                      Id = adminplacetb.Id,
                    //                                      CreateDt = adminplacetb.CreateDt,
                    //                                      CreateUser = adminplacetb.CreateUser,
                    //                                      UpdateDt = adminplacetb.UpdateDt,
                    //                                      UpdateUser = adminplacetb.UpdateUser,
                    //                                      DelYn = adminplacetb.DelYn,
                    //                                      DelDt = adminplacetb.DelDt,
                    //                                      DelUser = adminplacetb.DelUser,
                    //                                      AdminTbId = adminplacetb.AdminTbId,
                    //                                      PlaceId = adminplacetb.PlaceId
                    //                                  }).ToList();
                    //if (adminplace.Count() == 0)
                    //{
                    //    List<PlaceTb>? PlaceTbs = (from placetb in context.PlaceTbs.ToList()
                    //                              join list in placeidx
                    //                              on placetb.Id equals list
                    //                              where placetb.DelYn != 1
                    //                              select new PlaceTb()
                    //                              {
                    //                                  //Id = placetb.Id,
                    //                                  PlaceCd = placetb.PlaceCd,
                    //                                  ContractNum = placetb.ContractNum,
                    //                                  Name = placetb.Name,
                    //                                  Tel = placetb.Tel,
                    //                                  Note = placetb.Note,
                    //                                  Address = placetb.Address,
                    //                                  ContractDt = placetb.ContractDt,
                    //                                  PermMachine = placetb.PermMachine,
                    //                                  PermLift = placetb.PermLift,
                    //                                  PermFire = placetb.PermFire,
                    //                                  PermConstruct = placetb.PermConstruct,
                    //                                  PermNetwork = placetb.PermNetwork,
                    //                                  PermBeauty = placetb.PermBeauty,
                    //                                  PermSecurity = placetb.PermSecurity,
                    //                                  PermMaterial = placetb.PermMaterial,
                    //                                  PermEnergy = placetb.PermEnergy,
                    //                                  PermVoc = placetb.PermVoc,
                    //                                  CancelDt = placetb.CancelDt,
                    //                                  Status = placetb.Status,
                    //                                  CreateDt = placetb.CreateDt,
                    //                                  CreateUser = placetb.CreateUser,
                    //                                  UpdateDt = placetb.UpdateDt,
                    //                                  UpdateUser = placetb.UpdateUser,
                    //                                  DelDt = placetb.DelDt,
                    //                                  DelUser = placetb.DelUser,
                    //                                  DelYn = placetb.DelYn
                    //                              }).ToList();



                    var places = await context.PlaceTbs
                            .Where(m => placeidx.Contains(m.Id))
                            .ToListAsync();

                    foreach(var item in places)
                    {
                        foreach(var adminplace in item.AdminPlaceTbs)
                        {
                            adminplace.DelDt = DateTime.Now;
                            adminplace.DelUser = Name;
                            adminplace.DelYn = 1;
                            context.AdminPlaceTbs.Update(adminplace);
                        }
                        item.DelDt = DateTime.Now;
                        item.DelUser = Name;
                        item.DelYn = 1;
                        context.PlaceTbs.Update(item);
                    }
                    
                    return await context.SaveChangesAsync() > 0 ? true : false;
                        //return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
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
