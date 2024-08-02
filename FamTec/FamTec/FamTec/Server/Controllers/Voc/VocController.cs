using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Voc
{
    [Route("api/[controller]")]
    [ApiController]
    public class VocController : ControllerBase
    {
        private IVocService VocService;
        private ILogService LogService;

        public VocController(IVocService _vocservice, ILogService _logservice)
        {
            this.VocService = _vocservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장 민원 전체보기
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocList")]
        public async ValueTask<IActionResult> GetVocList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate, [FromQuery] int type, [FromQuery] int status, [FromQuery] int buildingid)
        {
            try
            {
                //DateTime start = DateTime.Now.AddDays(-1);
                //DateTime end = DateTime.Now;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<VocListDTO>? model = await VocService.GetVocList(HttpContext, StartDate, EndDate, type, status, buildingid);

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


        // 사업장 민원 상세
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailVoc")]
        public async ValueTask<IActionResult> GetDetailVoc([FromQuery]int VocId)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocDetailDTO?> dto = await VocService.GetVocDetail(HttpContext, VocId);

                if (dto is null)
                    return BadRequest();
                
                if (dto.code == 200)
                    return Ok(dto);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        // 민원 타입변경 - 미처리 --> 다른타입으로
        // -- 소켓 구현전
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateVocType")]
        public async ValueTask<IActionResult> UpdateVocType([FromBody]UpdateVocDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await VocService.UpdateVocService(HttpContext, dto);

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

    }
}
