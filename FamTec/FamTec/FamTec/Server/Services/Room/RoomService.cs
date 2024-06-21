using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Services.Room
{
    public class RoomService : IRoomService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        public RoomService(
            IBuildingInfoRepository _buildinginforepository,
            IFloorInfoRepository _floorinforepository,
            IRoomInfoRepository _roominforepository
            )
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;
            this.RoomInfoRepository = _roominforepository;
        }

        /// <summary>
        /// 공간 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<RoomDTO>?> AddRoomService(HttpContext? context, RoomDTO? dto)
        {
            if (dto is null)
                return new ResponseUnit<RoomDTO>() { message = "요청이 잘못되었습니다.", data = new RoomDTO(), code = 404 };

            if(context is null)
                return new ResponseUnit<RoomDTO>() { message = "요청이 잘못되었습니다.", data = new RoomDTO(), code = 404 };

            try
            {
                RoomTb roomtb = new RoomTb()
                {
                    Name = dto.Name,
                    FloorTbId = dto.FloorID,
                    CreateDt = DateTime.Now,
                    CreateUser = context.Items["Name"].ToString(),
                    UpdateDt = DateTime.Now,
                    UpdateUser = context.Items["Name"].ToString()
                };

                RoomTb? result = await RoomInfoRepository.AddAsync(roomtb);
                if(result is not null)
                {
                    return new ResponseUnit<RoomDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<RoomDTO>() { message = "요청이 처리되지 않았습니다.", data = new RoomDTO(), code = 204 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseUnit<RoomDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new RoomDTO(), code = 500 };
            }

        }

        /// <summary>
        /// 로그인한 사업장의 모든 공간정보 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<RoomListDTO>> GetRoomListService(HttpContext? context)
        {

            if (context is null)
                return new ResponseList<RoomListDTO>() { message = "요청이 잘못되었습니다.", data = new List<RoomListDTO>(), code = 404 };
            
            int? placeidx = Int32.Parse(context.Items["PlaceIdx"].ToString()); // 로그인한 사업장 인덱스

            List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(placeidx);

            List<RoomListDTO> result = new List<RoomListDTO>();

            if(buildinglist is [_, ..])
            {
                for (int i = 0; i < buildinglist.Count(); i++)
                {
                    List<FloorTb>? floortb = await FloorInfoRepository.GetFloorList(buildinglist[i].Id);
                    
                    if(floortb is [_, ..])
                    {
                        for (int j = 0; j < floortb.Count(); j++)
                        {
                            List<RoomTb>? roomtb = await RoomInfoRepository.GetRoomList(floortb[j].Id);

                            if(roomtb is [_, ..])
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

        }
    }
}
