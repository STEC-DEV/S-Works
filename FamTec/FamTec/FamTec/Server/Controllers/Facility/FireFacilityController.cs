using FamTec.Server.Services.Facility.Type.Fire;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class FireFacilityController : ControllerBase
    {
        private IFireFacilityService FireFacilityService;
        private ILogService LogService;

        public FireFacilityController(IFireFacilityService _firefacilityservice,
            ILogService _logservice)
        {
            this.FireFacilityService = _firefacilityservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFireFacility")]
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

                ResponseUnit<FacilityDTO>? model = await FireFacilityService.AddFireFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllFireFacility")]
        public async ValueTask<IActionResult> GetAllFireFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await FireFacilityService.GetFireFacilityListService(HttpContext);

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
        [Route("sign/DetailFireFacility")]
        public async ValueTask<IActionResult> DetailFireFacility([FromQuery] int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO?> model = await FireFacilityService.GetFireDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateFireFacility")]
        public async ValueTask<IActionResult> UpdateFireFacility([FromForm] FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<bool?> model = await FireFacilityService.UpdateFireFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteFireFacility")]
        public async ValueTask<IActionResult> DeleteFireFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await FireFacilityService.DeleteFireFacilityService(HttpContext, delIdx);
                
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
