using FamTec.Server.Services;
using FamTec.Server.Services.KakaoLog;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.KakaoLog
{
    [Route("api/[controller]")]
    [ApiController]
    public class KakaoLogController : ControllerBase
    {
        private IKakaoLogService KakaoLogService;
        private ILogService LogService;
        
        public KakaoLogController(IKakaoLogService _kakaologservice, ILogService _logservice)
        {
            this.KakaoLogService = _kakaologservice;
            this.LogService = _logservice;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetKakaoLogList")]
        public async ValueTask<IActionResult> GetKakaoLogList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<KakaoLogListDTO>? model = await KakaoLogService.GetKakaoLogListService(HttpContext);

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

    }
}
