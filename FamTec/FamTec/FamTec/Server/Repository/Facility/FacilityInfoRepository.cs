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

        /// <summary>
        /// 공간ID에 포함되어있는 전체 설비LIST 조회
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityTb>?> GetAllFacilityList(int? roomid)
        {
            try
            {
                if(roomid is not null)
                {
                    List<FacilityTb>? model = await context.FacilityTbs.Where(m => m.RoomTbId == roomid && m.DelYn != true).ToListAsync();

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
        /// 설비 ID로 단일 설비모델 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<FacilityTb?> GetFacilityInfo(int? id)
        {
            try
            {
                if(id is not null)
                {
                    FacilityTb? model = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true);
                    
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
        /// 설비 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateFacilityInfo(FacilityTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.FacilityTbs.Update(model);
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
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteFacilityInfo(FacilityTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.FacilityTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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

        public async ValueTask<List<MachineFacilityListDTO>?> GetPlaceMachineFacilityList(int? placeid)
        {
            try
            {
                if(placeid is not null)
                {
                    List<MachineFacilityListDTO> machinelist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                                join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                                on building.Id equals floortb.BuildingTbId
                                                                join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                                on floortb.Id equals roomtb.FloorTbId
                                                                join facilitytb in context.FacilityTbs.Where(m => m.Category == "기계" && m.DelYn != true).ToList()
                                                                on roomtb.Id equals facilitytb.RoomTbId
                                                                select new MachineFacilityListDTO
                                                                {
                                                                    Id = facilitytb.Id, // 설비인덱스
                                                                    Name = facilitytb.Name, // 설비명칭
                                                                    Type = facilitytb.Type,
                                                                    Num = facilitytb.Num, // 수량
                                                                    Unit = facilitytb.Unit, // 단위
                                                                    RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                                    RoomName = roomtb.Name, // 공간위치 이름
                                                                    StandardCapacity = facilitytb.StandardCapacity,
                                                                    EquipDT = facilitytb.EquipDt,
                                                                    LifeSpan = facilitytb.Lifespan,
                                                                    ChangeDT = facilitytb.ChangeDt
                                                                }).ToList();
                                                        
                    if(machinelist is [_, ..])
                    {
                        return machinelist;
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
