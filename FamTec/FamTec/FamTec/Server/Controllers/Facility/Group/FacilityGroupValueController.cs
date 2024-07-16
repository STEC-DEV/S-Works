using FamTec.Server.Services.Facility.Group;
using FamTec.Server.Services.Facility.Value;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupValueController : ControllerBase
    {
        private IFacilityValueService FacilityValueService;

        public FacilityGroupValueController(IFacilityValueService _facilityvalueservice)
        {
            this.FacilityValueService = _facilityvalueservice;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddValue")]
        public async ValueTask<IActionResult> AddGroupValue([FromBody] AddValueDTO dto)
        {
            ResponseUnit<AddValueDTO?> model = await FacilityValueService.AddValueService(HttpContext, dto);

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
        [HttpPut]
        [Route("sign/UpdateValue")]
        public async ValueTask<IActionResult> UpdateGroupValue([FromBody]UpdateValueDTO dto)
        {
            ResponseUnit<UpdateValueDTO?> model = await FacilityValueService.UpdateValueService(HttpContext, dto);
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
        [Route("sign/DeleteValue")]
        public async ValueTask<IActionResult> DeleteGroupValue([FromBody]int valueid)
        {
            ResponseUnit<bool?> model = await FacilityValueService.DeleteValueService(HttpContext, valueid);
            
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
