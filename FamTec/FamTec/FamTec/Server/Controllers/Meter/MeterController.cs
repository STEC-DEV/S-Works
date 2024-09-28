using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter
{
    [Route("api/[controller]")]
    [ApiController]
    //[ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class MeterController : ControllerBase
    {
        private IMeterService MeterService;
        private ILogService LogService;

        public MeterController(IMeterService _meterservice, ILogService _logservice)
        {
            this.MeterService = _meterservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 검침기 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMeter")]
        public async Task<IActionResult> AddMeter([FromBody]AddMeterDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(dto.Category.Equals("전기") && dto.ContractTbId == 0 || dto.ContractTbId is null)
                    return NoContent();

                ResponseUnit<AddMeterDTO> model = await MeterService.AddMeterService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
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
