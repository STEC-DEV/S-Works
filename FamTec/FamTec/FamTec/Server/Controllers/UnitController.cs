using FamTec.Server.Services.Unit;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private IUnitService UnitService;

        public UnitController(IUnitService _unitservice)
        {
            this.UnitService = _unitservice;
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
            ResponseList<UnitsDTO>? model = await UnitService.GetUnitList(HttpContext);
            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
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
        public async ValueTask<IActionResult> AddUnitInfo([FromBody]UnitsDTO dto)
        {
            ResponseUnit<UnitsDTO>? model = await UnitService.AddUnitService(HttpContext, dto);

            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteUnitInfo")]
        public async ValueTask<IActionResult> DeleteUnitInfo([FromBody]int unitid)
        {
            ResponseUnit<string?> model = await UnitService.DeleteUnitService(HttpContext, unitid);
            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateUnitInfo")]
        public async ValueTask<IActionResult> UpdateUnitInfo([FromBody]UnitsDTO dto)
        {
            ResponseUnit<UnitsDTO?> model = await UnitService.UpdateUnitService(HttpContext, dto);
            if (model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }



    }
}
