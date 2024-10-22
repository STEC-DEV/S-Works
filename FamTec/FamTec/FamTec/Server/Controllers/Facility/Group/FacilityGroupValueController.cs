using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Value;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupValueController : ControllerBase
    {
        private readonly IFacilityValueService FacilityValueService;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<FacilityGroupValueController> CreateBuilderLogger;

        public FacilityGroupValueController(IFacilityValueService _facilityvalueservice,
            ILogService _logservice,
            ConsoleLogService<FacilityGroupValueController> _createbuilderlogger)
        {
            this.FacilityValueService = _facilityvalueservice;

            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

   

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddValue")]
        public async Task<IActionResult> AddGroupValue([FromBody] AddValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.KeyID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Value))
                    return NoContent();

                ResponseUnit<AddValueDTO>? model = await FacilityValueService.AddValueService(HttpContext, dto).ConfigureAwait(false);

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
        [HttpPut]
        [Route("sign/UpdateValue")]
        public async Task<IActionResult> UpdateGroupValue([FromBody]UpdateValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.ItemValue))
                    return NoContent();

                ResponseUnit<UpdateValueDTO> model = await FacilityValueService.UpdateValueService(HttpContext, dto).ConfigureAwait(false);
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
        [HttpPut]
        [Route("sign/DeleteValue")]
        public async Task<IActionResult> DeleteGroupValue([FromBody]int valueid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await FacilityValueService.DeleteValueService(HttpContext, valueid).ConfigureAwait(false);

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

    }
}
