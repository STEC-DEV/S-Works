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
        public async ValueTask<FacilityTb?> AddAsync(FacilityTb model)
        {
            try
            {
                context.FacilityTbs.Add(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
                {
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
        public async ValueTask<List<FacilityTb>?> GetAllFacilityList(int roomid)
        {
            try
            {
                List<FacilityTb>? model = await context.FacilityTbs
                    .Where(m => m.RoomTbId == roomid && m.DelYn != true)
                    .ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;
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
        public async ValueTask<FacilityTb?> GetFacilityInfo(int id)
        {
            try
            {
                FacilityTb? model = await context.FacilityTbs.
                    FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true);
                    
                if (model is not null)
                    return model;
                else
                    return null;
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
        public async ValueTask<bool?> UpdateFacilityInfo(FacilityTb model)
        {
            try
            {
                context.FacilityTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
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
        public async ValueTask<bool?> DeleteFacilityInfo(FacilityTb model)
        {
            try
            {
                context.FacilityTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사업장에 속한 기계설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceMachineFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> machinelist =  (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                            join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                            on building.Id equals floortb.BuildingTbId
                                                            join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                            on floortb.Id equals roomtb.FloorTbId
                                                            join facilitytb in context.FacilityTbs.Where(m => m.Category == "기계" && m.DelYn != true).ToList()
                                                            on roomtb.Id equals facilitytb.RoomTbId
                                                            select new FacilityListDTO
                                                            {
                                                                Id = facilitytb.Id, // 설비인덱스
                                                                Name = facilitytb.Name, // 설비명칭
                                                                Type = facilitytb.Type,
                                                                Num = facilitytb.Num, // 수량
                                                                //Unit = facilitytb.Unit, // 단위
                                                                //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                                RoomName = roomtb.Name, // 공간위치 이름
                                                                StandardCapacity = facilitytb.StandardCapacity,
                                                                EquipDT = facilitytb.EquipDt,
                                                                LifeSpan = facilitytb.Lifespan,
                                                                ChangeDT = facilitytb.ChangeDt
                                                            }).ToList();

                if (machinelist is [_, ..])
                {
                    return machinelist;
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
        /// 사업장에 속한 전기설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceElectronicFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "전기" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if(electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 승강설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceLiftFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "승강" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 소방설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceFireFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "소방" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 건축설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceConstructFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "건축" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 통신설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceNetworkFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "통신" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 미화설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceBeautyFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "미화" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
        /// 사업장에 속한 보안설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityListDTO>?> GetPlaceSecurityFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = (from building in context.BuildingTbs.Where(m => m.PlaceTbId == placeid && m.DelYn != true).ToList()
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn != true).ToList()
                                                    on building.Id equals floortb.BuildingTbId
                                                    join roomtb in context.RoomTbs.Where(m => m.DelYn != true).ToList()
                                                    on floortb.Id equals roomtb.FloorTbId
                                                    join facilitytb in context.FacilityTbs.Where(m => m.Category == "보안" && m.DelYn != true).ToList()
                                                    on roomtb.Id equals facilitytb.RoomTbId
                                                    select new FacilityListDTO
                                                    {
                                                        Id = facilitytb.Id, // 설비인덱스
                                                        Name = facilitytb.Name, // 설비명칭
                                                        Type = facilitytb.Type, // 타입
                                                        Num = facilitytb.Num, // 수량
                                                        //RoomIdx = roomtb.Id, // 공간위치 인덱스
                                                        RoomName = roomtb.Name, // 공간위치 이름
                                                        StandardCapacity = facilitytb.StandardCapacity,
                                                        EquipDT = facilitytb.EquipDt,
                                                        LifeSpan = facilitytb.Lifespan,
                                                        ChangeDT = facilitytb.ChangeDt
                                                    }).ToList();

                if (electlist is [_, ..])
                {
                    return electlist;
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
}
