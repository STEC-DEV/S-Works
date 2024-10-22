using FamTec.Server.Services.Floor;
using FamTec.Shared.Server.DTO.Floor;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;
using FamTec.Server.Middleware;

namespace FamTec.Server.Controllers.Floor
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService FloorService;
        private readonly ILogService LogService;

        private readonly ConsoleLogService<FloorController> CreateBuilderLogger;

        public FloorController(IFloorService _floorservice,
            ILogService _logservice,
            ConsoleLogService<FloorController> _createbuilderlogger)
        {
            this.FloorService = _floorservice;

            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
