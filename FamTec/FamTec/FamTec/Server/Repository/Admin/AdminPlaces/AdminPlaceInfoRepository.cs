using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;
using Microsoft.EntityFrameworkCore;


namespace FamTec.Server.Repository.Admin.AdminPlaces
{
    public class AdminPlaceInfoRepository : IAdminPlacesInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public AdminPlaceInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 사업장 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddAsync(List<AdminPlaceTb>? model)
        {
            try
            {
                if (model is [_, ..])
                {
                    for (int i = 0; i < model.Count; i++)
                    {
                        context.AdminPlaceTbs.Add(model[i]);
                    }
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장 삭제
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteMyWorks(List<AdminPlaceTb>? modellist)
        {
            try
            {
                if (modellist is [_, ..])
                {
                    for (int i = 0; i < modellist.Count(); i++)
                    {
                        modellist[i].DelYn = true;
                        modellist[i].DelDt = DateTime.Now;

                        context.AdminPlaceTbs.Update(modellist[i]);
                    }

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

        /// <summary>
        /// 관리자에 해당하는 사업장리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<List<AdminPlaceTb>?> GetMyWorksList(int? adminid)
        {
            try
            {
                if(adminid is not null)
                {
                    List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs.Where(m => m.AdminTbId == adminid && m.DelYn != true).ToListAsync();

                    if(adminplacetb is [_, ..])
                        return adminplacetb;
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
        /// 관리자에 해당하는 사업장 리스트 출력
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<List<AdminPlaceDTO>?> GetMyWorks(int? adminid)
        {
            try
            {
                if(adminid is not null)
                {
                    List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs.Where(m => m.AdminTbId == adminid && m.DelYn != true).ToListAsync();

                    if(adminplacetb is [_, ..])
                    {
                        List<PlaceTb>? placetb = await context.PlaceTbs.ToListAsync();
                        if(placetb is [_, ..])
                        {

                            List<AdminPlaceDTO>? result = (from admin in adminplacetb
                                                   join place in placetb
                                                   on admin.PlaceId equals place.Id
                                                   where place.DelYn != true
                                                           select new AdminPlaceDTO
                                                   {
                                                      Id = place.Id,
                                                      PlaceCd = place.PlaceCd,
                                                      Name = place.Name,
                                                      Note = place.Note,
                                                      ContractNum = place.ContractNum,
                                                      ContractDt = place.ContractDt,
                                                      Status = place.Status
                                                   }).ToList();
                            if(result is [_, ..])
                            {
                                return result;
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
        /// 매니저 상세보기
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<DManagerDTO?> GetManagerDetails(int? adminidx)
        {
            try
            {
                if (adminidx is not null)
                {
                    AdminTb? admintb = await context.AdminTbs.FirstOrDefaultAsync(m => m.Id == adminidx && m.DelYn != true);

                    if (admintb is not null)
                    {
                        DepartmentTb? departmenttb = await context.DepartmentTbs.FirstOrDefaultAsync(m => m.Id == admintb.DepartmentTbId && m.DelYn != true);

                        if (departmenttb is not null)
                        {
                            UserTb? usertb = await context.UserTbs.FirstOrDefaultAsync(m => m.Id == admintb.UserTbId && m.DelYn != true);

                            if (usertb is not null)
                            {
                                DManagerDTO dto = new DManagerDTO
                                {
                                    UserId = usertb.UserId,
                                    Name = usertb.Name,
                                    Password = usertb.Password,
                                    Phone = usertb.Phone,
                                    Email = usertb.Email,
                                    Type = admintb.Type,
                                    Department = departmenttb.Name
                                };

                                return dto;
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

      


        /// <summary>
        /// 관리자사업장 리스트 모델에 해당하는 사업장 리스트들 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<List<PlaceTb>?> GetMyWorksDetails(List<AdminPlaceTb>? model)
        {
            try
            {
                if(model is [_, ..])
                {
                    List<PlaceTb>? result =  (from adminplacetb in model
                                            join placetb in context.PlaceTbs.Where(m => m.DelYn != true)
                                            on adminplacetb.PlaceId equals placetb.Id
                                            where adminplacetb.DelYn != true && placetb.DelYn != true
                                              select new PlaceTb
                                            {
                                                Id = placetb.Id,
                                                PlaceCd = placetb.PlaceCd,
                                                ContractNum = placetb.ContractNum,
                                                Name = placetb.Name,
                                                Tel = placetb.Tel,
                                                Note = placetb.Note,
                                                Address = placetb.Address,
                                                ContractDt = placetb.ContractDt,
                                                PermMachine = placetb.PermMachine,
                                                PermLift = placetb.PermLift,
                                                PermFire = placetb.PermFire,
                                                PermConstruct = placetb.PermConstruct,
                                                PermNetwork = placetb.PermNetwork,
                                                PermBeauty = placetb.PermBeauty,
                                                PermSecurity = placetb.PermSecurity,
                                                PermMaterial = placetb.PermMaterial,
                                                PermEnergy = placetb.PermEnergy,
                                                PermVoc = placetb.PermVoc,
                                                CancelDt = placetb.CancelDt,
                                                Status = placetb.Status,
                                                CreateDt = placetb.CreateDt,
                                                CreateUser = placetb.CreateUser,
                                                UpdateDt = placetb.UpdateDt,
                                                UpdateUser = placetb.UpdateUser,
                                                DelYn = placetb.DelYn,
                                                DelDt = placetb.DelDt,
                                                DelUser = placetb.DelUser
                                            }).ToList();

                    if (result is [_, ..])
                        return result;
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
        /// 사업장번호로 사업장 상세정보조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<PlaceDetailDTO?> GetWorksInfo(int? placeid)
        {
            try
            {
                if (placeid is not null)
                {
                    PlaceTb? place = await context.PlaceTbs.FirstOrDefaultAsync(m => m.Id == placeid && m.DelYn != true);

                    if (place is not null)
                    {

                        List<ManagerListDTO>? ManagerDTO = (from admintb in context.AdminTbs.ToList()
                                                            join adminplacetb in context.AdminPlaceTbs.Where(m => m.PlaceId == placeid).ToList()
                                                            on admintb.Id equals adminplacetb.AdminTbId
                                                            join usertb in context.UserTbs.ToList()
                                                            on admintb.UserTbId equals usertb.Id
                                                            join departmenttb in context.DepartmentTbs.ToList()
                                                            on admintb.DepartmentTbId equals departmenttb.Id
                                                            where (admintb.DelYn != true && adminplacetb.DelYn != true)
                                                            select new ManagerListDTO
                                                            {
                                                                Id = admintb.Id,
                                                                UserId = usertb.UserId,
                                                                Name = usertb.Name,
                                                                Department = departmenttb.Name
                                                            }).ToList();

                        PlaceDetailDTO PlaceDetail = new PlaceDetailDTO
                        {
                            PlaceInfo = new PlaceInfo
                            {
                                Id = place.Id,
                                PlaceCd = place.PlaceCd,
                                Name = place.Name,
                                Tel = place.Tel,
                                ContractNum = place.ContractNum,
                                ContractDt = place.ContractDt,
                                CancelDt = place.CancelDt,
                                Status = place.Status,
                                Note = place.Note
                            },

                            PlacePerm = new PlacePerm
                            {
                                Id = place.Id,
                                PermMachine = place.PermMachine,
                                PermLift = place.PermLift,
                                PermFire = place.PermFire,
                                PermConstruct = place.PermConstruct,
                                PermNetwork = place.PermNetwork,
                                PermBeauty = place.PermBeauty,
                                PermSecurity = place.PermSecurity,
                                PermMaterial = place.PermMaterial,
                                PermEnergy = place.PermEnergy,
                                PermVoc = place.PermVoc
                            },
                            ManagerList = ManagerDTO
                        };

                        return PlaceDetail;
                        
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
        /// 관리자 사업장 조회 - 사업장 INDEX
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<AdminPlaceTb?> GetWorksModelInfo(int? placeid)
        {
            try
            {
                if(placeid is not null)
                {
                    AdminPlaceTb? adminplacetb = await context.AdminPlaceTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.PlaceId == placeid);

                    if(adminplacetb is not null)
                    {
                        return adminplacetb;
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
