using FamTec.Server.Services.Facility.Type.Electronic;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectronicFacilityController : ControllerBase
    {
        private IElectronicFacilityService ElectronicFacilityService;

        public ElectronicFacilityController(IElectronicFacilityService _electronicfacilityservice)
        {
            this.ElectronicFacilityService = _electronicfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddElectronicFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] AddFacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<AddFacilityDTO>? model = await ElectronicFacilityService.AddElectronicFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllElectronicFacility")]
        public async ValueTask<IActionResult> GetAllElecFacility()
        {
            ResponseList<FacilityListDTO>? model = await ElectronicFacilityService.GetElectronicFacilityListService(HttpContext);

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
        [Route("sign/DetailElectronicFacility")]
        public async ValueTask<IActionResult> DetailElecFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await ElectronicFacilityService.GetElectronicDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateElectronicFacility")]
        public async ValueTask<IActionResult> UpdateElecFacility([FromForm] UpdateFacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await ElectronicFacilityService.UpdateElectronicFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteElectronicFacility")]
        public async ValueTask<IActionResult> DeleteElecFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await ElectronicFacilityService.DeleteElectronicFacilityService(HttpContext, delIdx);
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
