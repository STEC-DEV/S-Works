using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
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
        /// 해당사업장의 건물들의 모든 민원 반환
        /// </summary>
        /// <param name="buildinglist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<ListVoc>?> GetVocList(List<BuildingTb>? buildinglist, string? date)
        {
            try
            {
                if(buildinglist is [_, ..] && date is not null)
                {
                    List<ListVoc>? model =  (from bulidtbs in buildinglist
                                          join voctbs in context.VocTbs.Where(m => EF.Functions.Like(m.CreateDt, $"{date}%"))
                                          on bulidtbs.Id equals voctbs.BuildingTbId
                                          where bulidtbs.DelYn != true && voctbs.DelYn != true
                                        select new ListVoc
                                          {
                                             Id = voctbs.Id,
                                             Writer =voctbs.Name,
                                             Title = voctbs.Title,
                                             Type = voctbs.Type,
                                             Location = bulidtbs.Name,
                                             Status = voctbs.Status,
                                             Total_DT = voctbs.TotalTime.ToString(),
                                             Occur_DT = voctbs.CreateDt,
                                             Compelete_DT = voctbs.TotalTime
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
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
