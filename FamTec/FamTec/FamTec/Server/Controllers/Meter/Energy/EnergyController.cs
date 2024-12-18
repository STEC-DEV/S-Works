﻿using FamTec.Server.Middleware;
using FamTec.Server.Repository.Meter.Energy;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter.Energy;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter.Energy
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyController : ControllerBase
    {
        private readonly IEnergyService EnergyService;
        private readonly IEnergyInfoRepository EnergyInfoRepository;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<EnergyController> CreateBuilderLogger;


        public EnergyController(IEnergyService _energyservice,
            IEnergyInfoRepository _energyinforepository,
            ILogService _logservice,
            ConsoleLogService<EnergyController> _createbuilderlogger)
        {
            this.EnergyService = _energyservice;
            this.EnergyInfoRepository = _energyinforepository;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 에너지 일일 검침값 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/AddEnergy")]
        //public async Task<IActionResult> AddEnergy()
        public async Task<IActionResult> AddEnergy([FromBody]AddEnergyDTO dto)
        {
            try
            {
                //{
                //    "MeterID":1,
                //    "MeterDate":"2024-09-13",
                //    "Amount1":400,
                //    "Amount2":600,
                //    "Amount3":1500,
                //    "TotalAmount":2100
                //}

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MeterID is 0)
                    return NoContent();

                ResponseUnit<AddEnergyDTO>? model = await EnergyService.AddEnergyService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText(model.data?.ToString() ?? "");
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
        /// (1) - 일일검침
        /// 해당년도-월의 검침기별 일별 전체데이터 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllMonthRecord")]
        public async Task<IActionResult> GetAllMonthRecord()
        {
            try
            {
                DateTime ThisDate = DateTime.Now;
                int placeid = 1;

                var temp = await EnergyInfoRepository.GetMonthList(ThisDate, placeid);
                return Ok(temp);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/AddChargePrice")]
        public async Task<IActionResult> AddChargePrice()
        {
            try
            {
                EnergyMonthChargePriceDTO dto = new EnergyMonthChargePriceDTO();
                dto.TargetDate = DateTime.Now;
                dto.ChargePrice = 300000;

                var temp = await EnergyInfoRepository.AddChargePrice(dto, "용", 1);
                return Ok();

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
        /// (2) 계약종별
        /// 계약종별에 따른 월별 데이터
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllContractTypeMonthRecord")]
        public async Task<IActionResult> GetAllContractTypeMonthRecord()
        {
            try
            {
                DateTime ThisDate = DateTime.Now;
                int placeid = 1;

                var temp = await EnergyInfoRepository.GetContractTypeMonthList(ThisDate,  placeid);
                return Ok(temp);
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
        /// (3) 사용량비교
        /// 당월 / 전월 / 전년동월
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetContractTypeUseCompare")]
        public async Task<IActionResult> GetContractTypeUseCompare()
        {
            try
            {
                DateTime ThisDate = DateTime.Now;
                int placeid = 1;
                var temp = await EnergyInfoRepository.GetContractTypeUseCompare(ThisDate, placeid);
                return Ok(temp);

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




        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMeterMonthRecord")]
        public async Task<IActionResult> GetMeterMonthRecord()
        {
            try
            {
                DateTime ThisDate = DateTime.Now;
                int placeid = 1;
                List<int> MeterId = new List<int>() { 5, 6, 7 };

                var temp = await EnergyInfoRepository.GetMeterMonthList(ThisDate, MeterId, placeid);
                return Ok(temp);
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
