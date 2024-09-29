using FamTec.Server.Services.Floor;
using FamTec.Shared.Server.DTO.Floor;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Floor
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private IFloorService FloorService;
        private ILogService LogService;

        public FloorController(IFloorService _floorservice,
            ILogService _logservice)
        {
            this.FloorService = _floorservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFloor")]
        public async Task<IActionResult> AddFloorInfo([FromBody]FloorDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.BuildingTBID is null)
                    return NoContent();

                ResponseUnit<FloorDTO> model = await FloorService.AddFloorService(HttpContext, dto).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetFloorList")]
        public async Task<IActionResult> GetFloorList([FromQuery] int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FloorDTO> model = await FloorService.GetFloorListService(buildingid).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateFloor")]
        public async Task<IActionResult> UpdateFloor([FromBody] UpdateFloorDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.FloorID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<bool?> model = await FloorService.UpdateFloorService(HttpContext, dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteFloor")]
        public async Task<IActionResult> DeleteFloor([FromBody] List<int> idx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await FloorService.DeleteFloorService(HttpContext, idx).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 201)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
