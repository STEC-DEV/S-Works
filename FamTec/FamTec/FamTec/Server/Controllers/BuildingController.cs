using FamTec.Server.Services.Building;
using FamTec.Server.Services.Floor;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Floor;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace FamTec.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private IBuildingService BuildingService;
        private IFloorService FloorService;


        private SessionInfo session;

        public BuildingController(IBuildingService _buildingservice,
            IFloorService _floorservice)
        {
            this.BuildingService = _buildingservice;
            this.FloorService = _floorservice;


            this.session = new SessionInfo();
        }

        /// <summary>
        /// 사업장에 해당하는 건물리스트 출력 [수정완료]
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/MyBuildings")]
        public async ValueTask<IActionResult> SelectMyBuilding()
        {
            ResponseList<BuildinglistDTO>? model = await BuildingService.GetBuilidngListService(HttpContext);
            
            if(model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
            }
        }


        /// <summary>
        /// 사업장에 건물 추가 [수정완료]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign/AddBuilding")]
        public async ValueTask<IActionResult> InsertBuilding([FromBody]BuildingsDTO dto)
        {
            ResponseUnit<bool> model = await BuildingService.AddBuildingService(HttpContext, dto);

            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetFloorList")]
        public async ValueTask<IActionResult> GetFloorList([FromQuery]int buildingid)
        {
            ResponseList<FloorDTO>? model = await FloorService.GetFloorListService(buildingid);
            if(model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("DeleteFloor")]
        public async ValueTask<IActionResult> DeleteFloor([FromBody] List<int> idx)
        {
            ResponseModel<string>? model = await FloorService.DeleteFloorService(idx, session);
            return Ok(model);
        }

    

    }
}
