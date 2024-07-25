using FamTec.Server.Services.Facility.Type.Fire;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class FireFacilityController : ControllerBase
    {
        private IFireFacilityService FireFacilityService;

        public FireFacilityController(IFireFacilityService _firefacilityservice)
        {
            this.FireFacilityService = _firefacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFireFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<FacilityDTO>? model = await FireFacilityService.AddFireFacilityService(HttpContext, dto, files);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllFireFacility")]
        public async ValueTask<IActionResult> GetAllFireFacility()
        {
            ResponseList<FacilityListDTO>? model = await FireFacilityService.GetFireFacilityListService(HttpContext);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailFireFacility")]
        public async ValueTask<IActionResult> DetailFireFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await FireFacilityService.GetFireDetailFacilityService(HttpContext, facilityid);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateFireFacility")]
        public async ValueTask<IActionResult> UpdateFireFacility([FromForm] FacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await FireFacilityService.UpdateFireFacilityService(HttpContext, dto, files);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteFireFacility")]
        public async ValueTask<IActionResult> DeleteFireFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await FireFacilityService.DeleteFireFacilityService(HttpContext, delIdx);
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
