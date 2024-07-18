using FamTec.Server.Services.Facility.Type.Network;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkFacilityController : ControllerBase
    {
        private INetworkFacilityService NetworkFacilityService;

        public NetworkFacilityController(INetworkFacilityService _networkfacilityservice)
        {
            this.NetworkFacilityService = _networkfacilityservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddNetworkFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] AddFacilityDTO dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<AddFacilityDTO>? model = await NetworkFacilityService.AddNetworkFacilityService(HttpContext, dto, files);

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
        [Route("sign/GetAllNetworkFacility")]
        public async ValueTask<IActionResult> GetAllNetworkFacility()
        {
            ResponseList<FacilityListDTO>? model = await NetworkFacilityService.GetNetworkFacilityListService(HttpContext);

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
        [Route("sign/DetailNetworkFacility")]
        public async ValueTask<IActionResult> DetailNetworkFacility([FromQuery] int? facilityid)
        {
            ResponseUnit<FacilityDetailDTO?> model = await NetworkFacilityService.GetNetworkDetailFacilityService(HttpContext, facilityid);
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
        [Route("sign/UpdateNetworkFacility")]
        public async ValueTask<IActionResult> UpdateNetworkFacility([FromForm] UpdateFacilityDTO? dto, IFormFile? files)
        {
            ResponseUnit<bool?> model = await NetworkFacilityService.UpdateNetworkFacilityService(HttpContext, dto, files);
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
        [Route("sign/DeleteNetworkFacility")]
        public async ValueTask<IActionResult> DeleteNetworkFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await NetworkFacilityService.DeleteNetworkFacilityService(HttpContext, delIdx);
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
