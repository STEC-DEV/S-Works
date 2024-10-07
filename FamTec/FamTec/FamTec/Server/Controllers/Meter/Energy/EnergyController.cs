using FamTec.Server.Middleware;
using FamTec.Server.Repository.Meter.Energy;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter.Energy;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter.Energy
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyController : ControllerBase
    {
        private IEnergyService EnergyService;
        private ILogService LogService;
        private IEnergyInfoRepository EnergyInfoRepository;


        public EnergyController(IEnergyService _energyservice,
            IEnergyInfoRepository _energyinforepository,
            ILogService _logservice)
        {
            this.EnergyService = _energyservice;
            this.EnergyInfoRepository = _energyinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 에너지 일일 검침값 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpPost]
        [HttpGet]
        [Route("sign/AddEnergy")]
        public async Task<IActionResult> AddEnergy()
        //public async Task<IActionResult> AddEnergy([FromBody]AddEnergyDTO dto)
        {
            try
            {
                AddEnergyDTO dto = new AddEnergyDTO();
                dto.MeterID = 5;
                dto.MeterDate = DateTime.Now.AddDays(-3); // 검침일자
                dto.Amount1 = 500; // 소계
                dto.Amount2 = 700; // 중간부하
                dto.Amount3 = 1600; // 최대부하
                dto.TotalAmount = dto.Amount1 + dto.Amount2 + dto.Amount3;

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MeterID is 0)
                    return NoContent();

                ResponseUnit<AddEnergyDTO>? model = await EnergyService.AddEnergyService(HttpContext, dto);
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

        /// <summary>
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
                int placeid = 13;

                var temp = await EnergyInfoRepository.GetMonthList(ThisDate, placeid);
                return Ok(temp);

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


       

    }
}
