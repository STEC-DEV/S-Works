using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Facility
{
    public class FacilityInfoRepository : IFacilityInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FacilityInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 설비추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FacilityTb?> AddAsync(FacilityTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FacilityTbs.Add(model);
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

        public async ValueTask<List<FacilityListDTO>?> GetFacilityList(int? placeid)
        {
            try
            {
                if (placeid is not null)
                {
                    List<FacilityListDTO>? model = (from buildingtb in context.BuildingTbs
                                                    join floortb in context.FloorTbs
                                                    on buildingtb.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs
                                                    on roomtb.Id equals facilitytb.RoomTbid
                                                    where buildingtb.DelYn != true && floortb.DelYn != true && roomtb.DelYn != true && facilitytb.DelYn != true
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 형식
                                                        Ea = facilitytb.Ea, // 개수
                                                        RoomName = roomtb.Name, // 위치
                                                        StandardCapacity = facilitytb.StandardCapacity, // 규격용량
                                                        FacCreateDT = facilitytb.FacCreateDt, // 설치년월
                                                        LifeSpan = facilitytb.Lifespan, // 내용연수
                                                        FacUpdateDT = facilitytb.UpdateDt // 교체년월
                                                    }).ToList();

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

        public async ValueTask<FacilityDetailDTO?> GetDetailInfo(int? facilityId)
        {
            try
            {
                if (facilityId is not null)
                {

                    IQueryable<FacilityDetailDTO>? model = (from facilitytb in context.FacilityTbs.Where(m => m.Id == facilityId)
                                                          join roomtb in context.RoomTbs
                                                          on facilitytb.RoomTbid equals roomtb.Id
                                                          join floortb in context.FloorTbs
                                                          on roomtb.FloorTbId equals floortb.Id
                                                          join buildingtb in context.BuildingTbs
                                                          on floortb.BuildingTbId equals buildingtb.Id
                                                          where facilitytb.DelYn != true && roomtb.DelYn != true && floortb.DelYn != true && buildingtb.DelYn != true
                                                          select new FacilityDetailDTO
                                                          {
                                                              Id = facilitytb.Id,
                                                              Name = facilitytb.Name,
                                                              BuildingId = buildingtb.Id,
                                                              BuildingName = buildingtb.Name,
                                                              FloorId = floortb.Id,
                                                              FloorName = floortb.Name,
                                                              RoomId = roomtb.Id,
                                                              RoomName = roomtb.Name,
                                                              Ea = facilitytb.Ea,
                                                              Category = facilitytb.Category,
                                                              Type = facilitytb.Type,
                                                              LifeSpan = facilitytb.Lifespan,
                                                              FacCreateDT = facilitytb.FacCreateDt,
                                                              FacUpdateDT = facilitytb.FacUpdateDt,
                                                              Standard_capacity = facilitytb.StandardCapacity,
                                                              Standard_unit = facilitytb.StandardCapacityUnit
                                                          });

                    if (model is not null)
                        return await model.FirstOrDefaultAsync();
                    else
                        return null;
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

        


    }
}
