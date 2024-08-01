using FamTec.Server.Services.Facility.Type.Beauty;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeautyFacilityController : ControllerBase
    {
        private IBeautyFacilityService BeautyFacilityService;
        private ILogService LogService;

        public BeautyFacilityController(IBeautyFacilityService _beautyfacilityservice,
            ILogService _logservice)
        {
            this.BeautyFacilityService = _beautyfacilityservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 미화 설비 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBeautyFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if(HttpContext is null)
                    return BadRequest();

                if(files is not null) // 파일이 있으면 1MB 제한
                {
                    if(files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<FacilityDTO>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<FacilityDTO>? model = await BeautyFacilityService.AddBeautyFacilityService(HttpContext, dto, files);

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
        /// 사업장에 속한 미화설비 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllBeautyFacility")]
        public async ValueTask<IActionResult> GetAllBeautyFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await BeautyFacilityService.GetBeautyFacilityListService(HttpContext);

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
        /// 미화설비 상세조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailBeautyFacility")]
        public async ValueTask<IActionResult> DetailBeautyFacility([FromQuery] int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO?> model = await BeautyFacilityService.GetBeautyDetailFacilityService(HttpContext, facilityid);
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
        /// 미화설비 정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateBeautyFacility")]
        public async ValueTask<IActionResult> UpdateBeautyFacility([FromForm] FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null) // 파일이 있으면 1MB 제한
                {
                    if(files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<bool?> model = await BeautyFacilityService.UpdateBeautyFacilityService(HttpContext, dto, files);
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
        /// 미화설비 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteBeautyFacility")]
        public async ValueTask<IActionResult> DeleteBeautyFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await BeautyFacilityService.DeleteBeautyFacilityService(HttpContext, delIdx);
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
