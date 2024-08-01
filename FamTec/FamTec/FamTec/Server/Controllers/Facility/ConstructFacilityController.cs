using FamTec.Server.Services.Facility.Type.Contstruct;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructFacilityController : ControllerBase
    {
        private IConstructFacilityService ConstructFacilityService;
        private ILogService LogService;

        public ConstructFacilityController(IConstructFacilityService _constructfacilityservice,
            ILogService _logservice)
        {
            this.ConstructFacilityService = _constructfacilityservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddConstructFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null)
                {
                    if(files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<FacilityDTO>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<FacilityDTO>? model = await ConstructFacilityService.AddConstructFacilityService(HttpContext, dto, files);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllConstructFacility")]
        public async ValueTask<IActionResult> GetAllConstructFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await ConstructFacilityService.GetConstructFacilityListService(HttpContext);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailConstructFacility")]
        public async ValueTask<IActionResult> DetailConstructFacility([FromQuery] int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO?> model = await ConstructFacilityService.GetConstructDetailFacilityService(HttpContext, facilityid);
                
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateConstructFacility")]
        public async ValueTask<IActionResult> UpdateConstructFacility([FromForm] FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null)
                {
                    if(files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<bool?> model = await ConstructFacilityService.UpdateConstructFacilityService(HttpContext, dto, files);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteConstructFacility")]
        public async ValueTask<IActionResult> DeleteConstructFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await ConstructFacilityService.DeleteConstructFacilityService(HttpContext, delIdx);
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
