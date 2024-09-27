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
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class EnergyController : ControllerBase
    {
        private IEnergyService EnergyService;
        private ILogService LogService;

        public EnergyController(IEnergyService _energyservice,
            IEnergyInfoRepository _energyinforepository,
            ILogService _logservice)
        {
            this.EnergyService = _energyservice;
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
                dto.MeterDate = DateTime.Now.AddMonths(-1).AddYears(-1).AddDays(-1);
                dto.Amount1 = 200;
                dto.Amount2 = 500;
                dto.Amount3 = 600;
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
        /// 해당 년도-월의 품목별 모든일자 데이터 조회 - 전체
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMonthList")]
        public async Task<IActionResult> GetMonthList()
        //public async Task<IActionResult> GetMonthList([FromQuery] DateTime SearchDate)
        {
            try
            {
                DateOnly DaOnly = DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

                DateTime SearchDate = DateTime.Now.AddYears(-1).AddMonths(-1);

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DayEnergyDTO>? model = await EnergyService.GetMonthListService(HttpContext, SearchDate);

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
        /// 해당 년도-월의 품목별 모든일자 데이터 조회 - 선택된 것
        /// </summary>
        /// <param name="MeterId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMeterMonthList")]
        //public async Task<IActionResult> GetMeterMonthList()
        public async Task<IActionResult> GetMeterMonthList([FromQuery] DateTime SearchDate, [FromQuery] List<int> MeterId)
        {
            try
            {
                //DateTime SearchDate = DateTime.Now;
                //List<int> MeterId = new List<int>() { 5, 7 };

                if (HttpContext is null)
                    return BadRequest();

                if (MeterId is null || !MeterId.Any())
                    return NoContent();

                ResponseList<DayEnergyDTO>? model = await EnergyService.GetMonthSelectListService(HttpContext, SearchDate, MeterId);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


        /// <summary>
        /// 해당 년도의 월별 통계
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetYearList")]
        public async Task<IActionResult> GetYearList([FromQuery]int year)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<YearsTotalEnergyDTO>? model = await EnergyService.GetYearListService(HttpContext, year);
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
        /// 해당 년도의 선택된 검침기에 대한 월별 통계
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMeterYearList")]
        //public async Task<IActionResult> GetMeterYearList()
        public async Task<IActionResult> GetYearList([FromQuery] List<int> MeterId, [FromQuery] int year)
        {
            try
            {
                //List<int> MeterId = new List<int>() { 5, 6 };
                //int year = 2022;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<YearsTotalEnergyDTO>? model = await EnergyService.GetYearSelectListService(HttpContext, MeterId, year);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 전체출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDayList")]
        //public async Task<IActionResult> GetDayList()
        public async Task<IActionResult> GetDayList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate)
        {
            try
            {

                //DateTime StartDate = DateTime.Now.AddDays(-47);
                //DateTime EndDate = DateTime.Now;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DayEnergyDTO>? model = await EnergyService.GetDaysListService(HttpContext, StartDate, EndDate);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 전체출력
        /// </summary>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMeterDayList")]
        //public async Task<IActionResult> GetMeterDayList()
        public async Task<IActionResult> GetMeterDayList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate, [FromQuery] List<int> MeterId)
        {
            try
            {
                //DateTime StartDate = DateTime.Now.AddDays(-47);
                //DateTime EndDate = DateTime.Now;

                //List<int> MeterId = new List<int>() { 5 };

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DayEnergyDTO>? model = await EnergyService.GetDaysSelectListService(HttpContext, MeterId, StartDate, EndDate);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
