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
        /// 사용자재 상세보기 - 재고수량 있음.
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
        [HttpGet]
        [HttpPost]
        [Route("sign/UpdateUseMaterial")]
        //public async Task<IActionResult> UpdateUseMaterial()
        public async Task<IActionResult> UpdateUseMaterial([FromBody] UpdateMaintenanceMaterialDTO dto)
        {
            try
            {
                //UpdateMaintenanceMaterialDTO dto = new UpdateMaintenanceMaterialDTO();
                //dto.MaintanceID = 142;// 유지보수 인덱스
                //dto.UseMaintanceID = 80; // 사용자재 테이블 인덱스
                //dto.Num = 10;
                

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is 0)
                    return NoContent();
                if (dto.UseMaintanceID is 0)
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
