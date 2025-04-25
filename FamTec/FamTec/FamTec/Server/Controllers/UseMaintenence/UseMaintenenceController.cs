using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.UseMaintenence
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class UseMaintenenceController : ControllerBase
    {
        private readonly IUseMaintenenceService UseMaintenenceService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<UseMaintenenceController> CreateBuilderLogger; /* 콘솔로그 */

        public UseMaintenenceController(IUseMaintenenceService _usemaintenenceservice,
            ILogService _logservice,
            ConsoleLogService<UseMaintenenceController> _createbuilderlogger)
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
        public async Task<IActionResult> GetDetailUseMaterial([FromQuery][Required]int useid, [FromQuery][Required]int materialid, [FromQuery][Required]int roomid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (materialid is 0)
                    return NoContent();
                
                if (roomid is 0)
                    return NoContent();

                ResponseModel<UseMaterialDetailDTO>? model = await UseMaintenenceService.GetDetailUseMaterialService(useid, materialid, roomid).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.UseMaintanceID is 0)
                    return NoContent();

                ResponseModel<bool?> model = await UseMaintenenceService.UpdateDetailUseMaterialService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

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
