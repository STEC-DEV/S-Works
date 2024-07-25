using FamTec.Server.Services.Facility.Type.Security;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityFacilityController : ControllerBase
    {
        private ISecurityFacilityService SecurityFacilityService;


        public SecurityFacilityController(ISecurityFacilityService _securityfacilityservice)
        {
            this.SecurityFacilityService = _securityfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddSecurityFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<FacilityDTO>? model = await SecurityFacilityService.AddSecurityFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllSecurityFacility")]
        public async ValueTask<IActionResult> GetAllSecurityFacility()
        {
            ResponseList<FacilityListDTO>? model = await SecurityFacilityService.GetSecurityFacilityListService(HttpContext);

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
        [Route("sign/DetailSecurityFacility")]
        public async ValueTask<IActionResult> DetailSecurityFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await SecurityFacilityService.GetSecurityDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateSecurityFacility")]
        public async ValueTask<IActionResult> UpdateSecurityFacility([FromForm] FacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await SecurityFacilityService.UpdateSecurityFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteSecurityFacility")]
        public async ValueTask<IActionResult> DeleteFireFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await SecurityFacilityService.DeleteSecurityFacilityService(HttpContext, delIdx);
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
