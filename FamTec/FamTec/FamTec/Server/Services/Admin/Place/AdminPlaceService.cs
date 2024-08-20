using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Building;
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
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private ILogService LogService;

  
        public AdminPlaceService(IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            IBuildingInfoRepository _buildinginforepository,
            ILogService _logservice)
        {
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;
            this.BuildingInfoRepository = _buildinginforepository;

            this.LogService = _logservice;
        }

        

        /// <summary>
        /// 전체사업장 조회 
        ///     - 시스템관리자, 관리자 => 전체 사업장 조회
        ///     - 매니저 => 할당된 사업장 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <returns>ResponseList<AllPlaceDTO></returns>
        public async ValueTask<ResponseList<AllPlaceDTO>> GetAllWorksService(HttpContext context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? AdminId = Convert.ToString(context.Items["AdminIdx"]);
                if(String.IsNullOrWhiteSpace(AdminId))
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Role = Convert.ToString(context.Items["Role"]);
                if(String.IsNullOrWhiteSpace(Role))
                    return new ResponseList<AllPlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if(Role == "매니저")
                {
                    List<AdminPlaceTb>? adminplacetb = await AdminPlaceInfoRepository.GetMyWorksList(Int32.Parse(AdminId));
                    if(adminplacetb is [_, ..])
                    {
                        List<PlaceTb> placetb = new List<PlaceTb>();
                        for (int i = 0; i < adminplacetb.Count(); i++) 
                        {
                            PlaceTb? model = await PlaceInfoRepository.GetByPlaceInfo(adminplacetb[i].PlaceTbId);
                            
                            if(model is not null)
                                placetb.Add(model);
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
                                    Note = e?.Note,
                                    ContractNum = e?.ContractNum,
                                    ContractDt = e?.ContractDt,
                                    Status = e.Status
                                }).ToList(),
                                code = 200
                            };
                        }
                        else
                        {
                            return new ResponseList<AllPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                        }
                    }
                    else
                    {
                        return new ResponseList<AllPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
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
                        return new ResponseList<AllPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AllPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 관리자의 서비스 사업장목록 보기
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksService(int adminid)
        {
            try
            {
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
                    return new ResponseList<AdminPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AdminPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 관리자의 서비스 사업장목록 보기
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksList(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AdminPlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? adminidx = Convert.ToString(context.Items["AdminIdx"]);
                if(String.IsNullOrWhiteSpace(adminidx))
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

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
                    return new ResponseList<AdminPlaceDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AdminPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 관리자정보 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<ManagerListDTO>> GetAllManagerListService()
        {
            try
            {
                List<ManagerListDTO>? model = await AdminUserInfoRepository.GetAllAdminUserList();

                if(model is [_, ..])
                    return new ResponseList<ManagerListDTO> { message = "데이터가 정상 처리되었습니다.", data = model, code = 200 };
                else 
                    return new ResponseList<ManagerListDTO> { message = "조회된 결과가 없습니다.", data = null, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ManagerListDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장 등록
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="dto">사업장 추가 DTO</param>
        /// <returns>추가된 사업장ID</returns>
        public async ValueTask<ResponseUnit<int?>> AddPlaceService(HttpContext context, AddPlaceDTO dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<int?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<int?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                // UK 검사
                bool? UKCheck = await PlaceInfoRepository.PlaceUKCheck(dto.PlaceCd!);
                if (UKCheck != true)
                    return new ResponseUnit<int?> { message = "이미 사용한 이력이 있는 코드입니다.", data = null, code = 202 };

                PlaceTb? place = new PlaceTb();
                place.PlaceCd = dto.PlaceCd!;
                place.Name = dto.Name!;
                place.Tel = dto.Tel!;
                place.Address = dto.Address;
                place.ContractNum = dto.ContractNum;
                place.ContractDt = Convert.ToDateTime(dto.ContractDT);
                place.PermMachine = dto.PermMachine!.Value;
                place.PermLift = dto.PermLift!.Value;
                place.PermFire = dto.PermFire!.Value;
                place.PermConstruct = dto.PermConstruct!.Value;
                place.PermNetwork = dto.PermNetwork!.Value;
                place.PermBeauty = dto.PermBeauty!.Value;
                place.PermSecurity = dto.PermSecurity!.Value;
                place.PermMaterial = dto.PermMaterial!.Value;
                place.PermEnergy = dto.PermEnergy!.Value;
                place.PermVoc = dto.PermVoc!.Value;
                place.CreateDt = DateTime.Now;
                place.CreateUser = Creater;
                place.UpdateDt = DateTime.Now;
                place.UpdateUser = Creater;
                place.Status = dto.Status!.Value;
                place.Note = dto.Note;
                place.DepartmentTbId = dto.DepartmentID;

                PlaceTb? place_result = await PlaceInfoRepository.AddPlaceInfo(place);

                if (place_result is not null)
                    return new ResponseUnit<int?> { message = "요청이 정상 처리되었습니다.", data = place_result.Id, code = 200 };
                else
                    return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장 정보 수정
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="dto">수정할 DTO</param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UpdatePlaceDTO>> UpdatePlaceService(HttpContext context, UpdatePlaceDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UpdatePlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<UpdatePlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdatePlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                PlaceTb? model = await PlaceInfoRepository.GetByPlaceInfo(dto.PlaceInfo.Id!.Value);
                if(model is not null)
                {
                    model.PlaceCd = dto.PlaceInfo.PlaceCd!;
                    model.Name = dto.PlaceInfo.Name!;
                    model.Tel = dto.PlaceInfo.Tel!;
                    model.ContractNum = dto.PlaceInfo.ContractNum;
                    model.ContractDt = dto.PlaceInfo.ContractDt;
                    model.CancelDt = dto.PlaceInfo.CancelDt;
                    model.Status = dto.PlaceInfo.Status!.Value;
                    model.Note = dto.PlaceInfo.Note;
                    model.PermMachine = dto.PlacePerm.PermMachine!.Value;
                    model.PermElec = dto.PlacePerm.PermElec!.Value;
                    model.PermLift = dto.PlacePerm.PermLift!.Value;
                    model.PermFire = dto.PlacePerm.PermFire!.Value;
                    model.PermConstruct = dto.PlacePerm.PermConstruct!.Value;
                    model.PermNetwork = dto.PlacePerm.PermNetwork!.Value;
                    model.PermBeauty = dto.PlacePerm.PermBeauty!.Value;
                    model.PermSecurity = dto.PlacePerm.PermSecurity!.Value;
                    model.PermMaterial = dto.PlacePerm.PermMaterial!.Value;
                    model.PermEnergy = dto.PlacePerm.PermEnergy!.Value;
                    model.PermVoc = dto.PlacePerm.PermVoc!.Value;
                    model.CreateDt = DateTime.Now;
                    model.CreateUser = creater;
                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = creater;
                    model.DepartmentTbId = dto.PlaceInfo.DepartmentID;

                    bool? UpdatePlaceResult = await PlaceInfoRepository.EditPlaceInfo(model);
                    if(UpdatePlaceResult == true)
                    {
                        return new ResponseUnit<UpdatePlaceDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<UpdatePlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<UpdatePlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UpdatePlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 사업장정보 상세조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<PlaceDetailDTO>> GetPlaceService(int placeid)
        {
            try
            {
                PlaceDetailDTO? model = await AdminPlaceInfoRepository.GetWorksInfo(placeid);

                if (model is not null)
                    return new ResponseUnit<PlaceDetailDTO> { message = "요청이 정상 처리되었습니다.", data = model, code = 200};
                else
                    return new ResponseUnit<PlaceDetailDTO> { message = "잘못된 요청입니다.", data = null, code = 404 };
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<PlaceDetailDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

       

        /// <summary>
        /// 사업장에 관리자 추가
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placemanager">추가할 관리자 정보 DTO</param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AddPlaceManagerService(HttpContext context, AddPlaceManagerDTO<ManagerListDTO> placemanager)
        {
            try
            {
                if (placemanager is null)
                    return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (context is null)
                    return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                int placeid = placemanager.PlaceId!.Value;
                List<ManagerListDTO> placeManagers = placemanager.PlaceManager!;

                if (placeManagers is [_, ..])
                {
                    // 중복검사
                    foreach(var manager in placeManagers)
                    {
                        AdminPlaceTb? alreadyCheck = await AdminPlaceInfoRepository.GetPlaceAdminInfo(manager.Id!.Value, placeid);
                        if(alreadyCheck is not null)
                        {
                            return new ResponseUnit<bool?>() { message = "해당 관리자는 이미 포함되어있습니다.", data = false, code = 202 };
                        }
                    }

                    List<AdminPlaceTb> adminplace = new List<AdminPlaceTb>();

                    foreach (var manager in placeManagers)
                    {
                        adminplace.Add(new AdminPlaceTb
                        {
                            AdminTbId = manager.Id!.Value,
                            CreateDt = DateTime.Now,
                            CreateUser = Creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = Creater,
                            PlaceTbId = placeid
                        });
                    }

                    if (adminplace is [_, ..])
                    {
                        bool? result = await AdminPlaceInfoRepository.AddAsync(adminplace);

                        if (result == true)
                        {
                            return new ResponseUnit<bool?> { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else if (result == false)
                        {
                            return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                        }
                        else
                        {
                            return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 관리자에 사업장 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> AddManagerPlaceSerivce(HttpContext context, AddManagerPlaceDTO dto)
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
                    List<AdminPlaceTb>? placeadmintb = new List<AdminPlaceTb>();
                    for (int i = 0; i < dto.PlaceList.Count(); i++)
                    {
                        PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(dto.PlaceList[i]);

                        if (placetb is not null)
                        {
                            placeadmintb.Add(new AdminPlaceTb
                            {
                                AdminTbId = dto.AdminID!.Value,
                                CreateDt = DateTime.Now,
                                CreateUser = creater,
                                UpdateDt = DateTime.Now,
                                UpdateUser = creater,
                                PlaceTbId = dto.PlaceList[i]
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
        /// 해당 사업장에 관리자 삭제
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteManagerPlaceService(HttpContext context, AddPlaceManagerDTO<ManagerListDTO> dto)
        {
            try
            {
                if(dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<int> adminidx = dto.PlaceManager!.Select(m => m.Id!.Value).ToList();

                bool? RemoveResult = await AdminPlaceInfoRepository.RemoveAdminPlace(adminidx, dto.PlaceId!.Value);
                if(RemoveResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if(RemoveResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 404 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "존재하지 않는 관리자입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }

        }

        /// <summary>
        /// 사업장 자체를 삭제
        ///     - 사업장에 할당된 매니저가 있으면 삭제안됨.
        ///     - 사업장에 할당된 건물이 있으면 삭제안됨.
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placeidx">삭제할 사업장 인덱스</param>
        /// <returns>TRUE & FALSE</returns>
        public async ValueTask<ResponseUnit<bool>> DeletePlaceService(HttpContext context, List<int> placeidx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                // 해당 사업장인덱스와 AdminPlaceTb의 PlaceTbID 외래키로 검색해서 있는지 검사 - 삭제조건 [1]
                List<AdminPlaceTb>? adminplaceetb = await AdminPlaceInfoRepository.SelectPlaceAdminList(placeidx);
                if(adminplaceetb is not null)
                    return new ResponseUnit<bool>() { message = "해당 사업장에 할당되어있는 관리자가 있어 삭제가 불가능합니다.", data = false, code = 204 };

                // 해당 사업장인덱스와 BuildingTb의 PlaceId 외래키로 검색해서 있는지 검사 - 삭제조건 [2]
                List<BuildingTb>? buildingtb = await BuildingInfoRepository.SelectPlaceBuildingList(placeidx);
                if(buildingtb is not null)
                    return new ResponseUnit<bool>() { message = "해당 사업장에 할당되어있는 건물이 있어 삭제가 불가능합니다.", data = false, code = 204 };

                bool? DeleteResult = await PlaceInfoRepository.DeletePlaceList(creater, placeidx);
                if(DeleteResult == true)
                {
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placeid">사업장 ID</param>
        /// <returns></returns>
        public async ValueTask<ResponseList<ManagerListDTO>> NotContainManagerList(HttpContext context, int placeid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<ManagerListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                PlaceTb? PlaceCheck = await PlaceInfoRepository.GetByPlaceInfo(placeid);
                if(PlaceCheck is null)
                    return new ResponseList<ManagerListDTO>() { message = "없는 사업장입니다.", data = null, code = 404 };

                List<ManagerListDTO>? SelectList = await AdminUserInfoRepository.GetNotContainsAdminList(placeid);
                
                if (SelectList is [_, ..])
                    return new ResponseList<ManagerListDTO>() { message = "요청이 정상 처리되었습니다.", data = SelectList, code = 200 };
                else
                    return new ResponseList<ManagerListDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ManagerListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="adminid">관리자 ID</param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AdminPlaceDTO>> NotContainPlaceList(HttpContext context, int adminid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AdminPlaceDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 관리자 인지 검사
                AdminTb? adminTB = await AdminUserInfoRepository.GetAdminIdInfo(adminid);
                if(adminTB is null)
                    return new ResponseList<AdminPlaceDTO>() { message = "없는 관리자입니다.", data = null, code = 404 };

                List<AdminPlaceDTO>? SelectList = await AdminPlaceInfoRepository.GetNotContainsPlaceList(adminid);
                
                if (SelectList is [_, ..])
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 정상 처리되었습니다.", data = SelectList, code = 200 };
                else
                    return new ResponseList<AdminPlaceDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AdminPlaceDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


    }
}
