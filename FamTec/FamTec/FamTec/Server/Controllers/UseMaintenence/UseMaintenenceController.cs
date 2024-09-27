using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.UseMaintenence
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class UseMaintenenceController : ControllerBase
    {
        private IUseMaintenenceService UseMaintenenceService;
        private ILogService LogService;

        public UseMaintenenceController(IUseMaintenenceService _usemaintenenceservice, ILogService _logservice)
        {
            this.UseMaintenenceService = _usemaintenenceservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사용자재 상세보기
        /// </summary>
        /// <param name="useid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailUseMaterial")]
        public async Task<IActionResult> GetDetailUseMaterial([FromQuery]int useid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (useid is 0)
                    return NoContent();

                ResponseUnit<UseMaterialDetailDTO>? model = await UseMaintenenceService.GetDetailUseMaterialService(HttpContext, useid).ConfigureAwait(false);
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
        /// 사용자재 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpGet]
        [HttpPost]
        [Route("sign/UpdateUseMaterial")]
        public async Task<IActionResult> UpdateUseMaterial([FromBody] UpdateMaintenanceMaterialDTO dto)
        {
            try
            {
                //UpdateMaintenanceMaterialDTO dto = new UpdateMaintenanceMaterialDTO();
                //dto.MaintanceID = 102;// 유지보수 인덱스
                //dto.UseMaintanceID = 30; // 사용자재 테이블 인덱스
                //dto.MaterialID = 10; // 자재 인덱스
                //dto.RoomID = 2; // 공간 인덱스
                //dto.Num = 60;
                //dto.UnitPrice = 2000;
                //dto.TotalPrice = dto.Num * dto.UnitPrice; //  총금액 -- 입고면 금액을 받아야함.

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is 0)
                    return NoContent();
                if (dto.UseMaintanceID is 0)
                    return NoContent();
                if (dto.MaterialID is 0)
                    return NoContent();

                ResponseUnit<bool?> model = await UseMaintenenceService.UpdateDetailUseMaterialService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
