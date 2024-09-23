using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Value;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FamTec.Server.Controllers.Building.Group
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class BuildingGroupValueController : ControllerBase
    {
        private IBuildingValueService BuildingValueService;
        private ILogService LogService;

        public BuildingGroupValueController(IBuildingValueService _buildingvalueservice,
            ILogService _logservice)
        {
            this.BuildingValueService = _buildingvalueservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddValue")]
        public async ValueTask<IActionResult> AddGroupValue([FromBody] AddValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.KeyID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Value))
                    return NoContent();

                ResponseUnit<AddValueDTO> model = await BuildingValueService.AddValueService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateValue")]
        public async ValueTask<IActionResult> UpdateGroupValue([FromBody]UpdateValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.ItemValue))
                    return NoContent();

                ResponseUnit<UpdateValueDTO> model = await BuildingValueService.UpdateValueService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteValue")]
        public async ValueTask<IActionResult> DeleteGroupValue([FromBody]int valueid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await BuildingValueService.DeleteValueService(HttpContext, valueid);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
