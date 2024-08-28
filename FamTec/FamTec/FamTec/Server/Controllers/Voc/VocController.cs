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
        private readonly IVocService VocService;
        private readonly ILogService LogService;

        public VocController(IVocService _vocservice,
            ILogService _logservice)
        {
            this.VocService = _vocservice;
            this.LogService = _logservice;
        }

       
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/temp")]
        public async ValueTask<IActionResult> GetTemp([FromQuery]List<int> type)
        {
            return Ok(type);
        }

        /// <summary>
        /// 사업장 민원 전체보기 - 직원용 (월간)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocList")]
        public async ValueTask<IActionResult> GetVocList([FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid)
        {
            try
            {
                //List<int> typesArray = type.Split(',').Select(int.Parse).ToList();
                //List<int> type = new List<int>() { 0,1, 7 };
                //List<int> status = new List<int>() { 1, 2 };
                //List<int> buildingid = new List<int>() { 1 };
                
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllVocListDTO> model = await VocService.GetVocList(HttpContext, type, status, buildingid);
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
        /// 사업장 민원 필터 전체보기 - 직원용 (기간)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocFilterList")]
        public async ValueTask<IActionResult> GetVocFilterList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate, [FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid)
        {
            try
            {
                //DateTime StartDate = DateTime.Now.AddDays(-30);
                //DateTime EndDate = DateTime.Now;
                //List<int> type = new List<int>() { 0,1, 7 };
                //List<int> status = new List<int>() { 1, 2 };
                //List<int> buildingid = new List<int>() { 1 };

                if (HttpContext is null)
                   return BadRequest();

                if (type is null)
                    return NoContent();
                if (type.Count == 0)
                    return NoContent();

                if (status is null)
                    return NoContent();
                if (status.Count == 0)
                    return NoContent();

                if (buildingid is null)
                    return NoContent();
                if (buildingid.Count == 0)
                    return NoContent();

                ResponseList<VocListDTO>? model = await VocService.GetVocFilterList(HttpContext, StartDate, EndDate, type, status, buildingid);

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
        /// 민원 상세보기 - 직원용
        /// </summary>
        /// <param name="VocId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/VocInfo")]
        public async ValueTask<IActionResult> GetDetailVoc([FromQuery] int VocId)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocEmployeeDetailDTO> dto = await VocService.GetVocDetail(HttpContext, VocId);

                if (dto is null)
                    return BadRequest();

                if (dto.code == 200)
                    return Ok(dto);
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
        /// 민원타입 변경
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateVocType")]
        public async ValueTask<IActionResult> UpdateVocType([FromBody]UpdateVocDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.VocID is null)
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                ResponseUnit<bool?> model = await VocService.UpdateVocTypeService(HttpContext, dto);

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
