using FamTec.Server.Services;
using FamTec.Server.Services.Unit;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Unit
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private IUnitService UnitService;
        private ILogService LogService;

        public UnitController(IUnitService _unitservice,
            ILogService _logservice)
        {
            UnitService = _unitservice;
            LogService = _logservice;
        }

        /// <summary>
        /// 로그인한 사업장 단위 전부조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/SelectUnitList")]
        public async ValueTask<IActionResult> GetUnitList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<UnitsDTO>? model = await UnitService.GetUnitList(HttpContext);
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
        /// 로그인한 사업장 단위 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUnitInfo")]
        public async ValueTask<IActionResult> AddUnitInfo([FromBody] UnitsDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UnitsDTO>? model = await UnitService.AddUnitService(HttpContext, dto);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteUnitInfo")]
        public async ValueTask<IActionResult> DeleteUnitInfo([FromBody] int unitid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<string?> model = await UnitService.DeleteUnitService(HttpContext, unitid);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateUnitInfo")]
        public async ValueTask<IActionResult> UpdateUnitInfo([FromBody] UnitsDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UnitsDTO?> model = await UnitService.UpdateUnitService(HttpContext, dto);
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
