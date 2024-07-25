using FamTec.Server.Services.Facility.Type.Contstruct;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructFacilityController : ControllerBase
    {
        private IConstructFacilityService ConstructFacilityService;

        public ConstructFacilityController(IConstructFacilityService _constructfacilityservice)
        {
            this.ConstructFacilityService = _constructfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddConstructFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<FacilityDTO>? model = await ConstructFacilityService.AddConstructFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllConstructFacility")]
        public async ValueTask<IActionResult> GetAllConstructFacility()
        {
            ResponseList<FacilityListDTO>? model = await ConstructFacilityService.GetConstructFacilityListService(HttpContext);

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
        [Route("sign/DetailConstructFacility")]
        public async ValueTask<IActionResult> DetailConstructFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await ConstructFacilityService.GetConstructDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateConstructFacility")]
        public async ValueTask<IActionResult> UpdateConstructFacility([FromForm] FacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await ConstructFacilityService.UpdateConstructFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteConstructFacility")]
        public async ValueTask<IActionResult> DeleteConstructFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await ConstructFacilityService.DeleteConstructFacilityService(HttpContext, delIdx);
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
