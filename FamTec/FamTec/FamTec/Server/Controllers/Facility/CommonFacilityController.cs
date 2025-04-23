using FamTec.Server.Services;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonFacilityController : ControllerBase
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private HttpResponseMessage? HttpResponse;

        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<CommonFacilityController> CreateBuilderLogger; /* 콘솔로그 */
        
        public CommonFacilityController(IHttpClientFactory _httpclientfacotry,
            ILogService _logservice,
            ConsoleLogService<CommonFacilityController> _createbuilderlogger)
        {
            this.HttpClientFactory = _httpclientfacotry;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/RequestAPI")]
        public async Task<IActionResult> RequestAPI([FromQuery][Required] string searchData)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                var client = HttpClientFactory.CreateClient("RequestAPI");
                HttpResponse = await client.GetAsync($"getPrdctClsfcNoUslfsvc?serviceKey=%2Fqeu4TLyL2lnX5YQ%2Bx7tAM7jLFNb2pIviG4saBOKLm4ZZY6MNG5YIOlarnfzSn%2B0Ow2I8YUlWB2KF%2BfYsslQ8Q%3D%3D&numOfRows=1000&pageNo=1&type=json&prdctClsfcNoNm={searchData}");

                if (!HttpResponse.IsSuccessStatusCode)
                    return StatusCode((int)HttpResponse.StatusCode, "ApiClient 호출 실패");

                var content = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var data = JsonSerializer.Deserialize<APIResponse>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var model = new ResponseUnit<APIResponse>() { message = "요청이 정상처리되었습니다.", data = data, code = 200 };
                return Ok(model);
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
