using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Type.Lift;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiftFacilityController : ControllerBase
    {
        private ILiftFacilityService LiftFacilityService;
        private ILogService LogService;

        public LiftFacilityController(ILiftFacilityService _listfacilityservice,
            ILogService _logservice)
        {
            this.LiftFacilityService = _listfacilityservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddLiftFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null)
                {
                    if(files.Length > 1048576)
                    {
                        return Ok(new ResponseUnit<FacilityDTO>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<FacilityDTO>? model = await LiftFacilityService.AddLiftFacilityService(HttpContext, dto, files);
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
        [Route("sign/GetAllLiftFacility")]
        public async ValueTask<IActionResult> GetAllLiftFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await LiftFacilityService.GetLiftFacilityListService(HttpContext);
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
        [Route("sign/DetailLiftFacility")]
        public async ValueTask<IActionResult> DetailLiftFacility([FromQuery]int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO?> model = await LiftFacilityService.GetLiftDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateLiftFacility")]
        public async ValueTask<IActionResult> UpdateLiftFacility([FromForm] FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null)
                {
                    if(files.Length > 1048576)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<bool?> model = await LiftFacilityService.UpdateLiftFacilityService(HttpContext, dto, files);

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
        [Route("sign/DeletLiftFacility")]
        public async ValueTask<IActionResult> DeleteLiftFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await LiftFacilityService.DeleteLiftFacilityService(HttpContext, delIdx);
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
