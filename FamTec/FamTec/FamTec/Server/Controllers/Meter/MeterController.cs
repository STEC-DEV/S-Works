﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        private readonly IMeterService MeterService;

        private readonly ILogService LogService;
        private readonly ILogger<MeterController> BuilderLogger;

        public MeterController(IMeterService _meterservice,
            ILogService _logservice,
            ILogger<MeterController> _builderlogger)
        {
            this.MeterService = _meterservice;

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
        /// 검침기 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/AddMeter")]
        //public async Task<IActionResult> AddMeter()
        public async Task<IActionResult> AddMeter([FromBody]AddMeterDTO dto)
        {
            try
            {
                //AddMeterDTO dto = new AddMeterDTO();
                //dto.Name = "B검침기";
                //dto.Category = "전기";
                //dto.ContractTbId = 1;

                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(dto.Category.Equals("전기"))
                {
                    if(dto.ContractTbId == 0 || dto.ContractTbId is null)
                    {
                        return NoContent();
                    }
                }

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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }




    }
}
