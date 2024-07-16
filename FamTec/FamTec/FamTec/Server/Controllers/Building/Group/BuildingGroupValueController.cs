using FamTec.Server.Services.Building.Value;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupValueController : ControllerBase
    {
        private IBuildingValueService BuildingValueService;

        public BuildingGroupValueController(IBuildingValueService _buildingvalueservice)
        {
            this.BuildingValueService = _buildingvalueservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddValue")]
        public async ValueTask<IActionResult> AddGroupValue([FromBody] AddValueDTO dto)
        {
            ResponseUnit<AddValueDTO?> model = await BuildingValueService.AddValueService(HttpContext, dto);
            if(model is not null)
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
            ResponseUnit<UpdateValueDTO?> model = await BuildingValueService.UpdateValueService(HttpContext, dto);
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
            ResponseUnit<bool?> model = await BuildingValueService.DeleteValueService(HttpContext, valueid);

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
