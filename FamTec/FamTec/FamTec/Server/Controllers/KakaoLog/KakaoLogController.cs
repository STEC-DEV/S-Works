using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.KakaoLog;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.KakaoLog
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class KakaoLogController : ControllerBase
    {
        private readonly IKakaoLogService KakaoLogService;
        private readonly ILogService LogService;
        private readonly IKakaoService KakaoService;
        
        private readonly ConsoleLogService<KakaoLogController> CreateBuilderLogger;

        public KakaoLogController(IKakaoLogService _kakaologservice,
            ILogService _logservice,
            ConsoleLogService<KakaoLogController> _createbuilderlogger,
            IKakaoService _kakaoservice)
        {
            this.KakaoLogService = _kakaologservice;
            this.LogService = _logservice;
            this.KakaoService = _kakaoservice;

            // 콘솔로그
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 카카오 로그리스트
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="StartDate"></param>
        /// <param name="limit_day"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/KakaoSenderResult")]
        //public async Task<IActionResult> KakaoSenderResult()
        public async Task<IActionResult> KakaoSenderResult([FromQuery]int page, [FromQuery]int pagesize, [FromQuery] DateTime StartDate, [FromQuery] int limit_day)
        {
            try
            {
                //int page = 1;
                //int pagesize =50;
                //DateTime StartDate = DateTime.Now.AddDays(-50);
                //int limit_day = 100;

                if (HttpContext is null)
                    return BadRequest();

                if (page is 0)
                    return NoContent();

                if(pagesize is 0)
                    return NoContent();

                if (StartDate == DateTime.MinValue)
                    return NoContent();

                if(limit_day is 0)
                    return NoContent();

                ResponseList<KaKaoSenderResult>? model = await KakaoService.KakaoSenderResult(HttpContext, page, pagesize, StartDate, limit_day);
                
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

        /// <summary>
        /// 카카오 로그 리스트 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetKakaoLogList")]
        public async Task<IActionResult> GetKakaoLogList([FromQuery]int isSuccess)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<KakaoLogListDTO>? model = await KakaoLogService.GetKakaoLogListService(HttpContext, isSuccess).ConfigureAwait(false);

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
        /// 카카오 로그 기간별 리스트 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetKakaoDateLogList")]
        public async Task<IActionResult> GetKakaoDateLogList([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate, [FromQuery]int isSuccess)
        {
            try
            {
                //DateTime StartDate = DateTime.Now.AddDays(-10);
                //DateTime EndDate = DateTime.Now;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<KakaoLogListDTO>? model = await KakaoLogService.GetKakaoLogDateListService(HttpContext, StartDate, EndDate, isSuccess).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (StartDate == DateTime.MinValue)
                    return NoContent();

                if (EndDate == DateTime.MinValue)
                    return NoContent();

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
        /// 페이지네이션용 카카오 로그 카운트
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllKakaoLogCount")]
        public async Task<IActionResult> GetAllKakaoLogCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await KakaoLogService.GetKakaoLogCountService(HttpContext).ConfigureAwait(false);

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
        /// 카카오 로그 리스트 페이지네이션 반환
        /// </summary>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllPageNationKakaoLog")]
        public async Task<IActionResult> GetAllPageNationKakaoLog([FromQuery]int pagenum, [FromQuery]int pagesize)
        {
            try
            {
                if (pagesize == 0 || pagesize > 100)
                    return BadRequest(); // 사이즈 초과

                if (pagenum == 0)
                    return BadRequest(); // 잘못된 요청

                ResponseList<KakaoLogListDTO>? model = await KakaoLogService.GetKakaoLogPageNationListService(HttpContext, pagenum, pagesize).ConfigureAwait(false);
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
