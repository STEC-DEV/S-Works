using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.UseMaintenence
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class UseMaintenenceController : ControllerBase
    {
        private readonly IUseMaintenenceService UseMaintenenceService;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<UseMaintenenceController> CreateBuilderLogger;

        public UseMaintenenceController(IUseMaintenenceService _usemaintenenceservice,
            ILogService _logservice,
            ConsoleLogService<UseMaintenenceController> _createbuilderlogger
        )
        {
            this.UseMaintenenceService = _usemaintenenceservice;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// 사용자재 상세보기 - [여기]
        /// </summary>
        /// <param name="useid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailUseMaterial")]
        public async Task<IActionResult> GetDetailUseMaterial([FromQuery]int useid, [FromQuery]int materialid, [FromQuery]int roomid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (materialid is 0)
                    return NoContent();
                
                if (roomid is 0)
                    return NoContent();

                ResponseUnit<UseMaterialDetailDTO>? model = await UseMaintenenceService.GetDetailUseMaterialService(HttpContext, useid, materialid, roomid).ConfigureAwait(false);
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

        /// <summary>
        /// 사용자재 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        [Route("sign/UpdateUseMaterial")]
        public async Task<IActionResult> UpdateUseMaterial([FromBody] UpdateMaintenanceMaterialDTO dto)
        {
            try
            {
                //UpdateMaintenanceMaterialDTO dto = new UpdateMaintenanceMaterialDTO();
                //dto.MaintanceID = 142;// 유지보수 인덱스
                //dto.UseMaintanceID = 80; // 사용자재 테이블 인덱스
                //dto.Num = 10;
                

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.UseMaintanceID is 0)
                    return NoContent();
                
                ResponseUnit<bool?> model = await UseMaintenenceService.UpdateDetailUseMaterialService(HttpContext, dto).ConfigureAwait(false);

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
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
