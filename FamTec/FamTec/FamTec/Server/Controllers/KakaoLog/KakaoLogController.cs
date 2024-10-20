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
        private readonly ILogger<KakaoLogController> BuilderLogger;

        public KakaoLogController(IKakaoLogService _kakaologservice,
            ILogService _logservice,
            ILogger<KakaoLogController> _builderlogger)
        {
            this.KakaoLogService = _kakaologservice;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor(); // 색상 초기화
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 카카오 로그 리스트 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetKakaoLogList")]
        public async Task<IActionResult> GetKakaoLogList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<KakaoLogListDTO>? model = await KakaoLogService.GetKakaoLogListService(HttpContext).ConfigureAwait(false);

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
                CreateBuilderLogger(ex);
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

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger(ex);
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

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
