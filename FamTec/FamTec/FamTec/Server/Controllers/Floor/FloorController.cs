using FamTec.Server.Services.Floor;
using FamTec.Shared.Server.DTO.Floor;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;
using FamTec.Server.Middleware;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Floor
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService FloorService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<FloorController> CreateBuilderLogger; /* 콘솔로그 */

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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.BuildingTBID is null)
                    return NoContent();

                ResponseUnit<FloorDTO> model = await FloorService.AddFloorService(dto).ConfigureAwait(false);
                
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetFloorList")]
        public async Task<IActionResult> GetFloorList([FromQuery][Required] int buildingid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.FloorID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<bool?> model = await FloorService.UpdateFloorService(dto).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteFloor")]
        public async Task<IActionResult> DeleteFloor([FromBody][Required] List<int> idx)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseUnit<bool?> model = await FloorService.DeleteFloorService(idx).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
