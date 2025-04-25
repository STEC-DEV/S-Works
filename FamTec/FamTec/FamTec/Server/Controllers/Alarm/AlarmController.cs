using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Alarm;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Alarm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Alarm
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService AlarmService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<AlarmController> CreateBuilderLogger; /* 콘솔로그 */
        
        public AlarmController(IAlarmService _alarmservice,
            ILogService _logservice,
            ConsoleLogService<AlarmController> _createbuilderlogger)
        {
            this.AlarmService = _alarmservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 안읽은 알람리스트 전체 출력
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAlarmList")]
        public async Task<IActionResult> GetAlarmList()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<AlarmDTO>> model = await AlarmService.GetAllAlarmService().ConfigureAwait(false);
      
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
        /// 안읽은 알람리스트 2주안의 내용만 출력
        /// </summary>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAlarmDateList")]
        public async Task<IActionResult> GetAlarmDateList()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                DateTime StartDate = DateTime.Now;

                ResponseModel<List<AlarmDTO>> model = await AlarmService.GetAllAlarmByDateService(StartDate).ConfigureAwait(false);
                
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

        /// <summary>
        /// 알람 전체 읽음처리
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/AllAlarmDelete")]
        public async Task<IActionResult> AllAlarmDelete()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<bool?> model = await AlarmService.AllAlarmDelete().ConfigureAwait(false);
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
        /// 알람 개별 읽음 처리
        /// </summary>
        /// <param name="delId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/AlarmDelete")]
        public async Task<IActionResult> AlarmDelete([FromBody][Required]int delId)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<bool?> model = await AlarmService.AlarmDelete(delId).ConfigureAwait(false);
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

    }
}
