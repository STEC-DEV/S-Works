using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Place;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;

namespace FamTec.Server.Services.Admin.Place
{
    public class AdminPlaceService : IAdminPlaceService
    {
        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IAdminPlacesInfoRepository AdminPlaceInfoRepository;
        private readonly IAdminUserInfoRepository AdminUserInfoRepository;
        private ILogService LogService;

  
        public AdminPlaceService(IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            ILogService _logservice)
        {
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;
            this.LogService = _logservice;
        }
    
        /// <summary>
        /// 전체사업장 조회 - 매니저는 본인이 할당된 사업장만 조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<AllPlaceDTO>?> GetAllWorksService(HttpContext? context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다..", data = new List<AllPlaceDTO>(), code = 404 };

                string? AdminId = Convert.ToString(context.Items["AdminIdx"]);
                if(String.IsNullOrWhiteSpace(AdminId))
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다..", data = new List<AllPlaceDTO>(), code = 404 };

                string? Role = Convert.ToString(context.Items["Role"]);
                if(String.IsNullOrWhiteSpace(Role))
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다..", data = new List<AllPlaceDTO>(), code = 404 };
                

                if(Role == "매니저")
                {
                    List<AdminPlaceTb>? adminplacetb = await AdminPlaceInfoRepository.GetMyWorksList(Int32.Parse(AdminId));
                    
                    if(adminplacetb is [_, ..])
                    {
                        List<PlaceTb>? placetb = new List<PlaceTb>();
                        
                        for (int i = 0; i < adminplacetb.Count(); i++) 
                        {
                            PlaceTb? model = await PlaceInfoRepository.GetByPlaceInfo(adminplacetb[i].PlaceId);
                            if(model is not null)
                            {
                                placetb.Add(model);
                            }
                            else
                            {
                                return new ResponseList<AllPlaceDTO>() { message = "관리자 사업장에 해당하는 실제 사업장정보가 없습니다.", data = new List<AllPlaceDTO>(), code = 200 };
                            }
                        };

                        if(placetb is [_, ..])
                        {
                            return new ResponseList<AllPlaceDTO>()
                            {
                                message = "요청이 정상 처리되었습니다.",
                                data = placetb.Select(e => new AllPlaceDTO
                                {
                                    Id = e.Id,
                                    PlaceCd = e.PlaceCd,
                                    Name = e.Name,
                                    Note = e.Note,
                                    ContractNum = e.ContractNum,
                                    ContractDt = e.ContractDt,
                                    Status = e.Status
                                }).ToList(),
                                code = 200
                            };
                        }
                        else
                        {
                            return new ResponseList<AllPlaceDTO>() { message = "관리자 사업장에 해당하는 실제 사업장정보가 없습니다.", data = new List<AllPlaceDTO>(), code = 200 };
                        }
                    }
                    else
                    {
                        return new ResponseList<AllPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<AllPlaceDTO>(), code = 200 };
                    }
                }
                else
                {
                    List<PlaceTb>? model = await PlaceInfoRepository.GetAllList();
                    if (model is [_, ..])
                    {
                        return new ResponseList<AllPlaceDTO>()
                        {
                            message = "요청이 정상 처리되었습니다.",
                            data = model.Select(e => new AllPlaceDTO
                            {
                                Id = e.Id,
                                PlaceCd = e.PlaceCd,
                                Name = e.Name,
                                Note = e.Note,
                                ContractNum = e.ContractNum,
                                ContractDt = e.ContractDt,
                                Status = e.Status
                            }).ToList(),
                            code = 200
                        };
                    }
                    else
                    {
                        return new ResponseList<AllPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<AllPlaceDTO>(), code = 200 };
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AllPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<AllPlaceDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 관리자의 서비스 사업장목록 보기
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksService(int? adminid)
        {
            try
            {
                if(adminid is null)
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 잘못되었습니다.", data = new List<AdminPlaceDTO>(), code = 404 };

                List<AdminPlaceDTO>? model = await AdminPlaceInfoRepository.GetMyWorks(adminid);

                if (model is [_, ..])
                {
                    return new ResponseList<AdminPlaceDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model,
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<AdminPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<AdminPlaceDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AdminPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<AdminPlaceDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 관리자의 서비스 사업장목록 보기
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksList(HttpContext? context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 잘못되었습니다.", data = new List<AdminPlaceDTO>(), code = 404 };

                string? adminidx = Convert.ToString(context.Items["AdminIdx"]);
                if(String.IsNullOrWhiteSpace(adminidx))
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 잘못되었습니다.", data = new List<AdminPlaceDTO>(), code = 404 };

                List<AdminPlaceDTO>? model = await AdminPlaceInfoRepository.GetMyWorks(Int32.Parse(adminidx));

                if (model is [_, ..])
                {
                    return new ResponseList<AdminPlaceDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model,
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<AdminPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<AdminPlaceDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AdminPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<AdminPlaceDTO>(), code = 500 };
            }
        }


        /// <summary>
        /// 전체 관리자 리스트 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<ManagerListDTO>?> GetAllManagerListService()
        {
            try
            {
                List<ManagerListDTO>? model = await AdminUserInfoRepository.GetAllAdminUserList();

                if(model is [_, ..])
                    return new ResponseList<ManagerListDTO> { message = "데이터가 정상 처리되었습니다.", data = model, code = 200 };
                else 
                    return new ResponseList<ManagerListDTO> { message = "조회된 결과가 없습니다.", data = new List<ManagerListDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ManagerListDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<ManagerListDTO>(), code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<int?>> AddPlaceService(HttpContext? context, AddPlaceDTO? dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<int?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<int?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<int?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                PlaceTb? place = new PlaceTb
                {
                    PlaceCd = dto.PlaceCd,
                    Name = dto.Name,
                    Tel = dto.Tel,
                    Address = dto.Address,
                    ContractNum = dto.ContractNum,
                    ContractDt = Convert.ToDateTime(dto.ContractDT),
                    PermMachine = dto.PermMachine,
                    PermLift = dto.PermLift,
                    PermFire = dto.PermFire,
                    PermConstruct = dto.PermConstruct,
                    PermNetwork = dto.PermNetwork,
                    PermBeauty = dto.PermBeauty,
                    PermSecurity = dto.PermSecurity,
                    PermMaterial = dto.PermMaterial,
                    PermEnergy = dto.PermEnergy,
                    PermVoc = dto.PermVoc,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater
                };

                PlaceTb? place_result = await PlaceInfoRepository.AddPlaceInfo(place);

                if (place_result is not null)
                {
                    return new ResponseUnit<int?> { message = "요청이 정상 처리되었습니다.", data = place_result.Id, code = 200 };
                }
                else
                {
                    return new ResponseUnit<int?> { message = "요청이 처리되지 않았습니다.", data = null, code = 200 };
                }

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 404 };
            }
        }

      

        /// <summary>
        /// 사업장정보 상세조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<PlaceDetailDTO>?> GetPlaceService(int? placeid)
        {
            try
            {
                if(placeid is null)
                    return new ResponseUnit<PlaceDetailDTO> { message = "잘못된 요청입니다.", data = new PlaceDetailDTO(), code = 404 };

              
                PlaceDetailDTO? model = await AdminPlaceInfoRepository.GetWorksInfo(placeid);

                if (model is not null)
                    return new ResponseUnit<PlaceDetailDTO> { message = "요청이 정상 처리되었습니다.", data = model, code = 200};
                else
                    return new ResponseUnit<PlaceDetailDTO> { message = "데이터가 존재하지 않습니다.", data = new PlaceDetailDTO(), code = 200 };
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<PlaceDetailDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new PlaceDetailDTO(), code = 500 };
            }
        }

       

        /// <summary>
        /// 관리자에 사업장 추가
        /// </summary>
        /// <param name="placemanager"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> AddPlaceManagerService(HttpContext? context, AddPlaceManagerDTO<ManagerListDTO>? placemanager)
        {
            try
            {
                if (placemanager is null)
                    return new ResponseUnit<bool> { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (context is null)
                    return new ResponseUnit<bool> { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<bool> { message = "잘못된 요청입니다.", data = false, code = 404 };

                int placeid = placemanager.PlaceId;
                List<ManagerListDTO> placeManagers = placemanager.PlaceManager;

                if (placeManagers is [_, ..])
                {
                    List<AdminPlaceTb> adminplace = new List<AdminPlaceTb>();

                    foreach (var manager in placeManagers)
                    {
                        adminplace.Add(new AdminPlaceTb
                        {
                            AdminTbId = manager.Id,
                            CreateDt = DateTime.Now,
                            CreateUser = Creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = Creater,
                            PlaceId = placeid
                        });
                    }

                    if (adminplace is [_, ..])
                    {
                        bool? result = await AdminPlaceInfoRepository.AddAsync(adminplace);

                        if (result == true)
                        {
                            return new ResponseUnit<bool> { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else if (result == false)
                        {
                            return new ResponseUnit<bool> { message = "요청이 처리되지 않았습니다.", data = false, code = 401 };
                        }
                        else
                        {
                            return new ResponseUnit<bool> { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<bool> { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool> { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 관리자에 사업장 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> AddManagerPlaceSerivce(HttpContext? context, AddManagerPlaceDTO? dto)
        {
            try
            {
                if(dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if(context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(creater is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (dto.PlaceList is [_, ..])
                {
                    List<AdminPlaceTb?> placeadmintb = new List<AdminPlaceTb?>();
                    for (int i = 0; i < dto.PlaceList.Count(); i++)
                    {
                        PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(dto.PlaceList[i]);

                        if (placetb is not null)
                        {
                            placeadmintb.Add(new AdminPlaceTb
                            {
                                AdminTbId = dto.AdminID,
                                CreateDt = DateTime.Now,
                                CreateUser = creater,
                                UpdateDt = DateTime.Now,
                                UpdateUser = creater,
                                PlaceId = dto.PlaceList[i]
                            });
                        }
                    }

                    if(placeadmintb is [_, ..])
                    {
                        bool? result = await AdminPlaceInfoRepository.AddAsync(placeadmintb);
                        if(result == true)
                        {
                            return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };

                        }
                        else if(result == false)
                        {
                            return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 200 };

                        }
                        else
                        {
                            return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };

                        }
                    }
                    else
                    {
                        return new ResponseUnit<bool>() { message = "데이터가 잘못되었습니다.", data = true, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }  
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 사업장 완전삭제
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> DeleteManagerPlaceService(HttpContext? context,List<int>? placeidx)
        {
            try
            {
                if(placeidx is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                if (placeidx.Count == 0)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? deleter = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                for (int i = 0; i < placeidx.Count(); i++)
                {
                    AdminPlaceTb? chktb = await AdminPlaceInfoRepository.GetWorksModelInfo(placeidx[i]);

                    if(chktb is not null)
                    {
                        return new ResponseUnit<bool>() { message = "할당된 사업장이 존재합니다.", data = false, code = 200 };
                    }
                }

                for (int i = 0; i < placeidx.Count(); i++) 
                {
                    // 모델조회
                    PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(placeidx[i]);
                    placetb!.DelYn = true;
                    placetb!.DelDt = DateTime.Now;
                    placetb!.DelUser = deleter;

                    // 삭제
                    bool? result = await PlaceInfoRepository.DeletePlaceInfo(placetb);
                    if (result != true)
                    {
                        return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                    }
                }

                return new ResponseUnit<bool>() { message = "삭제완료.", data = true, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }

        }

        /// <summary>
        /// 여기가 맞나
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> DeletePlaceService(HttpContext? context, List<int>? placeidx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (placeidx is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (placeidx.Count == 0)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? Name = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Name))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                bool? result = await PlaceInfoRepository.DeletePlaceList(Name, placeidx);

                if (result == true)
                {
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if (result == false)
                {
                    return new ResponseUnit<bool>() { message = "할당된 사업장이 존재합니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = true, code = 404 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = true, code = 500 };
            }
        }

    
    }
}
