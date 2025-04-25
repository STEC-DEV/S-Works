using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Building.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupController : ControllerBase
    {
        private readonly IBuildingGroupService GroupService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<BuildingGroupController> CreateBuilderLogger; /* 콘솔로그 */

        public BuildingGroupController(IBuildingGroupService _groupservice,
            ILogService _logservice,
            ConsoleLogService<BuildingGroupController> _createbuilderlogger)
        {
            this.GroupService = _groupservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddGroup")]
        public async Task<IActionResult> AddGroup([FromBody] AddGroupInfoDTO dto)
        {
            try
            {

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseModel<AddGroupInfoDTO> model = await GroupService.AddBuildingGroupInfoService(dto).ConfigureAwait(false);
                
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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBuildingGroup")]
        public async Task<IActionResult> AddBuildingGroup([FromBody] List<AddGroupDTO> dto)
        {
            try
            {
                if (dto is null)
                    return NoContent();

                foreach(AddGroupDTO group in dto)
                {
                    if(group.BuildingIdx == 0)
                        return NoContent();
                    if(String.IsNullOrWhiteSpace(group.Name))
                        return NoContent();

                    if (group.AddGroupKey is [_, ..])
                    {
                        foreach (var key in group.AddGroupKey)
                        {
                            if (String.IsNullOrWhiteSpace(key.Name))
                                return NoContent();

                            if (key.ItemValues is [_, ..])
                            {
                                foreach (var value in key.ItemValues)
                                {
                                    if (String.IsNullOrWhiteSpace(value.Values))
                                        return NoContent();
                                }
                            }
                        }
                    }
                }

                ResponseModel<bool> model = await GroupService.AddBuildingGroupService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 201)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 건물의 하위정보 전체조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetBuildingGroup")]
        public async Task<IActionResult> GetDetailBuilding([FromQuery][Required]int buildingid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                ResponseModel<List<GroupListDTO?>> model = await GroupService.GetBuildingGroupListService(buildingid).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 건물 그룹정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async Task<IActionResult> UpdateBuildingGroup([FromBody] UpdateGroupDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (dto.GroupId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.GroupName))
                    return NoContent();

                ResponseModel<bool?> model = await GroupService.UpdateGroupNameService(dto).ConfigureAwait(false);
                
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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async Task<IActionResult> DeleteBuildingGroup([FromBody][Required]int groupid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                ResponseModel<bool?> model = await GroupService.DeleteGroupService(groupid).ConfigureAwait(false);
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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
