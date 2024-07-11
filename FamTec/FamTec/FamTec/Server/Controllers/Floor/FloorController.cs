using FamTec.Server.Services.Floor;
using FamTec.Shared.Server.DTO.Floor;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Floor
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private IFloorService FloorService;

        public FloorController(IFloorService _floorservice)
        {
            this.FloorService = _floorservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFloor")]
        public async ValueTask<IActionResult> AddFloorInfo([FromBody]FloorDTO dto)
        {
            ResponseUnit<FloorDTO>? model = await FloorService.AddFloorService(HttpContext, dto);
            if (model is not null)
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetFloorList")]
        public async ValueTask<IActionResult> GetFloorList([FromQuery] int buildingid)
        {
            ResponseList<FloorDTO>? model = await FloorService.GetFloorListService(buildingid);
            if (model is not null)
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateFloor")]
        public async ValueTask<IActionResult> UpdateFloor([FromBody] UpdateFloorDTO? dto)
        {
            ResponseUnit<bool?> model = await FloorService.UpdateFloorService(HttpContext, dto);
            if (model is not null)
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteFloor")]
        public async ValueTask<IActionResult> DeleteFloor([FromBody] List<int> idx)
        {
            ResponseUnit<int?> model = await FloorService.DeleteFloorService(HttpContext, idx);
            if(model is not null)
            {
                if(model.code == 200)
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

    }
}
