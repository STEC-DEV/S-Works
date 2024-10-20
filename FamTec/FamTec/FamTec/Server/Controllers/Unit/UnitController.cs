using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Unit;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Unit
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService UnitService;
        
        private readonly ILogService LogService;
        private readonly ILogger<UnitController> BuilderLogger;

        public UnitController(IUnitService _unitservice,
            ILogService _logservice,
            ILogger<UnitController> _builderlogger)
        {
            this.UnitService = _unitservice;
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
        /// 로그인한 사업장 단위 전부조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/SelectUnitList")]
        public async Task<IActionResult> GetUnitList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<UnitsDTO> model = await UnitService.GetUnitList(HttpContext).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 로그인한 사업장 단위 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUnitInfo")]
        public async Task<IActionResult> AddUnitInfo([FromBody] UnitsDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                ResponseUnit<UnitsDTO>? model = await UnitService.AddUnitService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 201)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteUnitInfo")]
        public async Task<IActionResult> DeleteUnitInfo([FromBody] List<int> unitid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (unitid is null)
                    return NoContent();

                if (unitid.Count == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await UnitService.DeleteUnitService(HttpContext, unitid).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateUnitInfo")]
        public async Task<IActionResult> UpdateUnitInfo([FromBody] UnitsDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.Id is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                ResponseUnit<UnitsDTO> model = await UnitService.UpdateUnitService(HttpContext, dto).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
