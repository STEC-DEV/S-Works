using FamTec.Server.Services.Building;
using FamTec.Server.Services.Facility.Machine;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineFacilityController : ControllerBase
    {
        private IMachineFacilityService MachineFacilityService;

        public MachineFacilityController(IMachineFacilityService _machinefacilityservice)
        {
            this.MachineFacilityService = _machinefacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMachineFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] AddFacilityDTO dto, [FromForm]IFormFile? files)
        {
            ResponseUnit<AddFacilityDTO>? model = await MachineFacilityService.AddMachineFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllMachineFacility")]
        public async ValueTask<IActionResult> GetAllMachineFacility()
        {
            ResponseList<MachineFacilityListDTO>? model = await MachineFacilityService.GetMachineFacilityListService(HttpContext);

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
        [Route("sign/DetailMachineFacility")]
        public async ValueTask<IActionResult> DetailMachineFacility([FromQuery]int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await MachineFacilityService.GetMachineDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateMachineFacility")]
        public async ValueTask<IActionResult> UpdateMachineFacility([FromForm]UpdateFacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await MachineFacilityService.UpdateMachineFacilityService(HttpContext, dto, files);
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
