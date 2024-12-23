﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Alarm;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Alarm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Alarm
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService AlarmService;
        private readonly ILogService LogService;

        // 콘솔로그
        private readonly ConsoleLogService<AlarmController> CreateBuilderLogger;
        
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AlarmDTO> model = await AlarmService.GetAllAlarmService(HttpContext).ConfigureAwait(false);
      
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
                DateTime StartDate = DateTime.Now;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AlarmDTO> model = await AlarmService.GetAllAlarmByDateService(HttpContext, StartDate).ConfigureAwait(false);
                
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AlarmService.AllAlarmDelete(HttpContext).ConfigureAwait(false);
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
        /// 알람 개별 읽음 처리
        /// </summary>
        /// <param name="delId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/AlarmDelete")]
        public async Task<IActionResult> AlarmDelete([FromBody]int delId)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AlarmService.AlarmDelete(HttpContext, delId).ConfigureAwait(false);
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
