using FamTec.Server.Services.Facility;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private IFacilityService FacilityService;

        public FacilityController(IFacilityService _facilityservice)
        {
            this.FacilityService = _facilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFacility")]
        public async ValueTask<IActionResult> AddFacility([FromBody] AddFacilityDTO dto)
        {
            ResponseUnit<AddFacilityDTO>? model = await FacilityService.AddFacilityService(HttpContext, dto);

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
        [Route("sign/GetAllFacility")]
        public async ValueTask<IActionResult> GetAllFacility()
        {
            ResponseList<FacilityListDTO>? model = await FacilityService.GetFacilityListService(HttpContext);

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
        [Route("sign/GetDetailFacility")]
        public async ValueTask<IActionResult> GetDetailFacility([FromQuery]int? facilityId)
        {
            ResponseUnit<FacilityDetailDTO>? model = await FacilityService.GetDetailService(facilityId);

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
        [Route("sign/UpdateFacility")]
        public async ValueTask<IActionResult> GetUpdateFacility([FromBody]FacilityDetailDTO dto)
        {
            return Ok();
        }


    }
}
