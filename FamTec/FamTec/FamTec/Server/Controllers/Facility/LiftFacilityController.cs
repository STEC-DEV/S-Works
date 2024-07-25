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

        public LiftFacilityController(ILiftFacilityService _listfacilityservice)
        {
            this.LiftFacilityService = _listfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddLiftFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm]IFormFile files)
        {
            ResponseUnit<FacilityDTO>? model = await LiftFacilityService.AddLiftFacilityService(HttpContext, dto, files);
            if(model is not null)
            {
                if(model.code == 200)
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
        [Route("sign/GetAllLiftFacility")]
        public async ValueTask<IActionResult> GetAllLiftFacility()
        {
            ResponseList<FacilityListDTO>? model = await LiftFacilityService.GetLiftFacilityListService(HttpContext);
            if(model is not null)
            {
                if(model.code == 200)
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
        [Route("sign/DetailLiftFacility")]
        public async ValueTask<IActionResult> DetailLiftFacility([FromQuery]int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await LiftFacilityService.GetLiftDetailFacilityService(HttpContext, facilityid);
            if(model is not null)
            {
                if(model.code == 200)
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
        [Route("sign/UpdateLiftFacility")]
        public async ValueTask<IActionResult> UpdateLiftFacility([FromForm] FacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await LiftFacilityService.UpdateLiftFacilityService(HttpContext, dto, files);

            if(model is not null)
            {
                if(model.code == 200)
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
        [Route("sign/DeletLiftFacility")]
        public async ValueTask<IActionResult> DeleteLiftFacility([FromBody] List<int>delIdx)
        {
            ResponseUnit<int?> model = await LiftFacilityService.DeleteLiftFacilityService(HttpContext, delIdx);
            if(model is not null)
            {
                if(model.code == 200)
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
