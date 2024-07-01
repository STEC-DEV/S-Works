using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Place;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Floor;
using Microsoft.IdentityModel.Abstractions;
using System.Collections.Generic;

namespace FamTec.Server.Services.Floor
{
    public class FloorService : IFloorService
    {
        private readonly IFloorInfoRepository FloorInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;


        public FloorService(IFloorInfoRepository _floorinforepository, IBuildingInfoRepository _buildinginforepository)
        {
            this.FloorInfoRepository = _floorinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
        }

        /// <summary>
        /// 층 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<FloorDTO>?> AddFloorService(HttpContext? context, FloorDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };

                BuildingTb? tokenck = await BuildingInfoRepository.GetBuildingInfo(dto.BuildingTBID);
                if (tokenck is null)
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };


                FloorTb? model = new FloorTb()
                {
                    Name = dto.Name,
                    CreateDt = DateTime.Now,
                    CreateUser = creator,
                    BuildingTbId = dto.BuildingTBID
                };

                FloorTb? result = await FloorInfoRepository.AddAsync(model);
                if (result is not null)
                    return new ResponseUnit<FloorDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };
            }
            catch(Exception ex)
            {
                return new ResponseUnit<FloorDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FloorDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 건물에 속해있는 층 리스트 반환
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<FloorDTO>?> GetFloorListService(int? buildingtbid)
        {
            try
            {
                if(buildingtbid is not null)
                {
                    List<FloorTb>? model = await FloorInfoRepository.GetFloorList(buildingtbid);

                    if(model is [_, ..])
                    {
                        return new ResponseList<FloorDTO>()
                        {
                            message = "요청이 정상 처리되었습니다.",
                            data = model.Select(e => new FloorDTO
                            {
                                FloorID = e.Id,
                                Name = e.Name,
                                BuildingTBID = e.BuildingTbId
                            }).ToList(),
                            code = 200
                        };
                    }
                    else
                    {
                        return new ResponseList<FloorDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FloorDTO>(), code = 200 };
                    }
                }
                else
                {
                    return new ResponseList<FloorDTO>() { message = "잘못된 요청입니다.", data = new List<FloorDTO>(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseList<FloorDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<FloorDTO>(), code = 500 };
            }
        }
    }
}





