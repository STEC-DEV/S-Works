using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Floor;

namespace FamTec.Server.Services.Floor
{
    public class FloorService : IFloorService
    {
        private readonly IFloorInfoRepository FloorInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private ILogService LogService;

        public FloorService(IFloorInfoRepository _floorinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IRoomInfoRepository _roominforepository,
            ILogService _logservice)
        {
            this.FloorInfoRepository = _floorinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.RoomInfoRepository = _roominforepository;

            this.LogService = _logservice;
        }

        /// <summary>
        /// 층 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<FloorDTO>> AddFloorService(HttpContext context, FloorDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };

                DateTime ThisDate = DateTime.Now;

                BuildingTb? BuildingInfo = await BuildingInfoRepository.GetBuildingInfo(dto.BuildingTBID!.Value).ConfigureAwait(false);
                if (BuildingInfo is null)
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };

                FloorTb? model = new FloorTb()
                {
                    Name = dto.Name!,
                    CreateDt = ThisDate,
                    CreateUser = creator,
                    UpdateDt = ThisDate,
                    UpdateUser = creator,
                    BuildingTbId = dto.BuildingTBID.Value
                };
                
                FloorTb? result = await FloorInfoRepository.AddAsync(model).ConfigureAwait(false);

                if (result is not null)
                    return new ResponseUnit<FloorDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<FloorDTO>() { message = "잘못된 요청입니다.", data = new FloorDTO(), code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FloorDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FloorDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 건물에 속해있는 층 리스트 반환
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async Task<ResponseList<FloorDTO>> GetFloorListService(int buildingtbid)
        {
            try
            {
                List<FloorTb>? model = await FloorInfoRepository.GetFloorList(buildingtbid).ConfigureAwait(false);
                
                if(model != null && model.Any())
                {
                    var ListData = model.Select(e => new FloorDTO
                    {
                        FloorID = e.Id,
                        Name = e.Name,
                        BuildingTBID = e.BuildingTbId
                    }).ToList();

                    return new ResponseList<FloorDTO>() { message = "요청이 정상 처리되었습니다.", data = ListData, code = 200 };
                }

                return new ResponseList<FloorDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FloorDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<FloorDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<FloorDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 층수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateFloorService(HttpContext context, UpdateFloorDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                FloorTb? model = await FloorInfoRepository.GetFloorInfo(dto.FloorID!.Value).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "존재하지 않는 데이터입니다.", data = null, code = 404 };

                model.Name = dto.Name!;
                model.UpdateDt = ThisDate;
                model.UpdateUser = creater;

                bool? FloorUpdateResult = await FloorInfoRepository.UpdateFloorInfo(model).ConfigureAwait(false);
                return FloorUpdateResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        // 층삭제
        // 층에 물려있는 공간이 있으면 삭제 XX
        public async Task<ResponseUnit<bool?>> DeleteFloorService(HttpContext context, List<int> del)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (del is null || del.Count == 0)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 삭제검사 - Check
                foreach(int FloorId in del)
                {
                    bool? DelCheck = await FloorInfoRepository.DelFloorCheck(FloorId).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "해당 층을 참조하고있는 데이터가 있어 삭제가 불가능합니다.", data = null, code = 201 };
                }

                bool? DeleteResult = await FloorInfoRepository.DeleteFloorInfo(del, creater).ConfigureAwait(false);
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


    }
}





