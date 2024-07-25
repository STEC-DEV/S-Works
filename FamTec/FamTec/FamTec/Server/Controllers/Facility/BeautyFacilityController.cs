using FamTec.Server.Services.Facility.Type.Beauty;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeautyFacilityController : ControllerBase
    {
        private IBeautyFacilityService BeautyFacilityService;

        public BeautyFacilityController(IBeautyFacilityService _beautyfacilityservice)
        {
            this.BeautyFacilityService = _beautyfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBeautyFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<FacilityDTO>? model = await BeautyFacilityService.AddBeautyFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllBeautyFacility")]
        public async ValueTask<IActionResult> GetAllBeautyFacility()
        {
            ResponseList<FacilityListDTO>? model = await BeautyFacilityService.GetBeautyFacilityListService(HttpContext);

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
        [Route("sign/DetailBeautyFacility")]
        public async ValueTask<IActionResult> DetailBeautyFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await BeautyFacilityService.GetBeautyDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateBeautyFacility")]
        public async ValueTask<IActionResult> UpdateBeautyFacility([FromForm] FacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await BeautyFacilityService.UpdateBeautyFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteBeautyFacility")]
        public async ValueTask<IActionResult> DeleteBeautyFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await BeautyFacilityService.DeleteBeautyFacilityService(HttpContext, delIdx);
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
