using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Voc
{
    public class VocInfoRepository : IVocInfoRpeository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public VocInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<VocTb?> AddAsync(VocTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.VocTbs.Add(model);
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
        /// 로그인한 사업장에 속한 건물들에 대한 모든 민원리스트 반환
        /// </summary>
        /// <param name="buildinglist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<VocTb>?> GetVocList(List<BuildingTb>? buildinglist, string? date)
        {
            return null;
            //try
            //{
            //    if (String.IsNullOrWhiteSpace(date))
            //        return null;
                
            //    if(buildinglist is [_, ..])
            //    {
            //        List<VocTb>? model = (from buildingtbs in buildinglist
            //                              join voctbs in context.VocTbs.Where(m => EF.Functions.Like(m.CreateDt, $"{date}%"))
            //                              on buildingtbs.Id equals voctbs.BuildingTbId
            //                              where buildingtbs.DelYn != true && voctbs.DelYn != true
            //                              select new VocTb
            //                              {
            //                                  Id = voctbs.Id,
            //                                  Name = voctbs.Name,
            //                                  Title = voctbs.Title,
            //                                  Content = voctbs.Content,
            //                                  Phone = voctbs.Content,
            //                                  Status = voctbs.Status,
            //                                  Type = voctbs.Type,
            //                                  CreateDt = voctbs.CreateDt,
            //                                  CreateUser=voctbs.CreateUser,
            //                                  UpdateDt = voctbs.UpdateDt,
            //                                  UpdateUser = voctbs.UpdateUser,
            //                                  DelDt = voctbs.DelDt,
            //                                  DelUser = voctbs.DelUser,
            //                                  DelYn = voctbs.DelYn,
            //                                  Image1 = voctbs.Image1,
            //                                  Image2 = voctbs.Image2,
            //                                  Image3 = voctbs.Image3,
            //                                  CompleteTime = voctbs.CompleteTime,
            //                                  TotalTime = voctbs.TotalTime,
            //                                  BuildingTbId =voctbs.BuildingTbId
            //                              }).ToList();
            //        if (model is [_, ..])
            //            return model;
            //        else
            //            return null;
            //    }
            //    else
            //    {
            //        return null;
            //    }

                /*
                if(buildinglist is [_, ..] && date is not null)
                {
                    List<VocDTO>? model =  (from bulidtbs in buildinglist
                                          join voctbs in context.VocTbs.Where(m => EF.Functions.Like(m.CreateDt, $"{date}%"))
                                          on bulidtbs.Id equals voctbs.BuildingTbId
                                          where bulidtbs.DelYn != true && voctbs.DelYn != true
                                        select new VocDTO
                                        {
                                                Id = voctbs.Id,
                                                Location = bulidtbs.Name,
                                                Type = voctbs.Type,
                                                Writer = voctbs.Name,
                                                Tel = voctbs.Phone,
                                                Title = voctbs.Title,
                                                Content = voctbs.Content,
                                                Status = voctbs.Status,
                                                CreateDT = voctbs.CreateDt,
                                                CompleteDT = voctbs.CompleteTime,
                                                TotalDT = voctbs.TotalTime.ToString()
                                            }).ToList();

                    
                    if (model is [_, ..])
                        return model;

                    else
                        return new List<ListVoc>();
                }
                else
                {
                    return null;
                }
                */
            //}
            //catch(Exception ex)
            //{
            //    LogService.LogMessage(ex.ToString());
            //    throw new ArgumentNullException();
            //}
        }
    }
}
