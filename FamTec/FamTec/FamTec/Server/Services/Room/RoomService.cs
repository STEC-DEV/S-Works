using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Room;

namespace FamTec.Server.Services.Room
{
    public class RoomService : IRoomService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private ILogService LogService;

        public RoomService(
            IBuildingInfoRepository _buildinginforepository,
            IFloorInfoRepository _floorinforepository,
            IRoomInfoRepository _roominforepository,
            ILogService _logService)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;
            this.RoomInfoRepository = _roominforepository;

            this.LogService = _logService;
        }

        /// <summary>
        /// 공간 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<RoomDTO>> AddRoomService(HttpContext context, RoomDTO dto)
        {
            try
            {
                if (dto is null || context is null)
                    return new ResponseUnit<RoomDTO>() { message = "요청이 잘못되었습니다.", data = new RoomDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<RoomDTO>() { message = "요청이 잘못되었습니다.", data = new RoomDTO(), code = 404 };

                DateTime ThisDate = DateTime.Now;

                RoomTb roomtb = new RoomTb()
                {
                    Name = dto.Name!,
                    FloorTbId = dto.FloorID!.Value,
                    CreateDt = ThisDate,
                    CreateUser = creater,
                    UpdateDt = ThisDate,
                    UpdateUser = creater
                };

                RoomTb? result = await RoomInfoRepository.AddAsync(roomtb).ConfigureAwait(false);
                if(result is not null)
                    return new ResponseUnit<RoomDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<RoomDTO>() { message = "요청이 처리되지 않았습니다.", data = new RoomDTO(), code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<RoomDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new RoomDTO(), code = 500 };
            }

        }

        /// <summary>
        /// 로그인한 사업장의 모든 공간정보 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<RoomListDTO>> GetRoomListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<RoomListDTO>() { message = "요청이 잘못되었습니다.", data = new List<RoomListDTO>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<RoomListDTO>() { message = "요청이 잘못되었습니다.", data = new List<RoomListDTO>(), code = 404 };


                List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(PlaceIdx)).ConfigureAwait(false);

                List<RoomListDTO> result = new List<RoomListDTO>();

                if (buildinglist is [_, ..])
                {
                    for (int i = 0; i < buildinglist.Count(); i++)
                    {
                        List<FloorTb>? floortb = await FloorInfoRepository.GetFloorList(buildinglist[i].Id).ConfigureAwait(false);

                        if (floortb is [_, ..])
                        {
                            for (int j = 0; j < floortb.Count(); j++)
                            {
                                List<RoomTb>? roomtb = await RoomInfoRepository.GetRoomList(floortb[j].Id).ConfigureAwait(false);

                                if (roomtb is [_, ..])
                                {
                                    for (int k = 0; k < roomtb.Count(); k++)
                                    {
                                        result.Add(new RoomListDTO()
                                        {
                                            RoomID = roomtb[k].Id,
                                            RoomName = roomtb[k].Name,
                                            BuildingID = buildinglist[i].Id,
                                            BuildingName = buildinglist[i].Name,
                                            FloorID = floortb[j].Id,
                                            FloorName = floortb[j].Name,
                                            CreateDT = roomtb[k].CreateDt
                                        });
                                    }
                                }
                            }
                        }
                    }

                    if (result is [_, ..])
                    {
                        return new ResponseList<RoomListDTO>() { message = "요청이 정상 처리되었습니다.", data = result, code = 200 };
                    }
                    else
                    {
                        return new ResponseList<RoomListDTO>() { message = "등록된 데이터가 없습니다.", data = result, code = 200 };
                    }
                }
                else
                {
                    return new ResponseList<RoomListDTO>() { message = "등록된 건물 정보가 없습니다.", data = new List<RoomListDTO>(), code = 200 };
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<RoomListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 공간ID로 공간명칭 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> GetRoomNameService(HttpContext context, int roomid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(roomid is 0)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                RoomTb? model = await RoomInfoRepository.GetRoomInfo(roomid).ConfigureAwait(false);
                if (model is not null)
                    return new ResponseUnit<string?>() { message = "요청이 정상 처리되었습니다.", data = model.Name, code = 200 };
                else
                    return new ResponseUnit<string?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 204 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 공간 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateRoomService(HttpContext context, UpdateRoomDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                RoomTb? model = await RoomInfoRepository.GetRoomInfo(dto.RoomId!.Value).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Name = dto.Name!;
                model.UpdateDt = ThisDate;
                model.UpdateUser = creater;

                bool? UpdateRoomResult = await RoomInfoRepository.UpdateRoomInfo(model).ConfigureAwait(false);
                return UpdateRoomResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = true, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = true, code = 500 };
            }
        }

        /// <summary>
        /// 공간정보 삭제 - 하위 포함되어있는거 있으면 삭제안됨
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteRoomService(HttpContext context, List<int> del)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (del is null || del is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                foreach(int index in del)
                {
                    bool? DelCheck = await RoomInfoRepository.DelRoomCheck(index).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "해당 정보를 참조하는 데이터가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                //foreach(int index in del)
                //{
                //    List<FacilityTb>? FacilityList = await FacilityInfoRepository.GetAllFacilityList(index);
                //    if (FacilityList is [_, ..])
                //        return new ResponseUnit<bool?>() { message = "해당 공간에 속한 장치정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };

                //    bool? Inventory = await RoomInfoRepository.RoomDeleteCheck(Convert.ToInt32(placeid), index);
                //    if (Inventory != true)
                //        return new ResponseUnit<bool?>() { message = "해당 공간에 속한 자재가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                //}


                bool? DeleteResult = await RoomInfoRepository.DeleteRoomInfo(del, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 해당하는 전체건물 - 전체층 - 전체공간 Group
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<PlaceRoomListDTO>> GetPlaceAllGroupRoomInfo(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PlaceRoomListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<PlaceRoomListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<PlaceRoomListDTO>? model = await RoomInfoRepository.GetPlaceAllGroupRoomInfo(Int32.Parse(placeid)).ConfigureAwait(false);
                if (model is [_, ..])
                    return new ResponseList<PlaceRoomListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<PlaceRoomListDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<PlaceRoomListDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<PlaceRoomListDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


    }
}
