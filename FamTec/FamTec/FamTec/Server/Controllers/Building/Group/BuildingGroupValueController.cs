using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Value;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupValueController : ControllerBase
    {
        private readonly IBuildingValueService BuildingValueService;
        private readonly ILogService LogService;
        private readonly ILogger<BuildingGroupValueController> BuilderLogger;

        public BuildingGroupValueController(IBuildingValueService _buildingvalueservice,
            ILogService _logservice,
            ILogger<BuildingGroupValueController> _builderlogger)
        {
            this.BuildingValueService = _buildingvalueservice;
            
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor(); // 색상 초기화
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddValue")]
        public async Task<IActionResult> AddGroupValue([FromBody] AddValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.KeyID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Value))
                    return NoContent();

                ResponseUnit<AddValueDTO> model = await BuildingValueService.AddValueService(HttpContext, dto).ConfigureAwait(false);
                
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateValue")]
        public async Task<IActionResult> UpdateGroupValue([FromBody]UpdateValueDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.ItemValue))
                    return NoContent();

                ResponseUnit<UpdateValueDTO> model = await BuildingValueService.UpdateValueService(HttpContext, dto).ConfigureAwait(false);
                
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteValue")]
        public async Task<IActionResult> DeleteGroupValue([FromBody]int valueid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await BuildingValueService.DeleteValueService(HttpContext, valueid).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
