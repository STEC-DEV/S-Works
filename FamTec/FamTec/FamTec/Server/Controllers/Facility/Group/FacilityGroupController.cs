using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Facility.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupController : ControllerBase
    {
        private readonly IFacilityGroupService GroupService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<FacilityGroupController> CreateBuilderLogger; /* 콘솔로그 */

        public FacilityGroupController(IFacilityGroupService _groupservice,
            ILogService _logservice,
            ConsoleLogService<FacilityGroupController> _createbuilderlogger)
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

                if (dto.FacilityIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseModel<AddGroupInfoDTO> model = await GroupService.AddFacilityGroupInfoService(dto).ConfigureAwait(false);
                
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

        /// <summary>
        /// 설비 그룹 - 키 - 값 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFacilityGroup")]
        public async Task<IActionResult> AddFacilityGroup([FromBody] AddGroupDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.Id is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                // 그룹의 - 키(값) 단위 검사
                if (dto.AddGroupKey is [_, ..]) {
                    foreach (var group in dto.AddGroupKey)
                    {
                        if (String.IsNullOrWhiteSpace(group.Name))
                            return NoContent();
                    }
                }

                ResponseModel<AddGroupDTO> model = await GroupService.AddFacilityGroupService(dto).ConfigureAwait(false);

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
        [Route("sign/AddFacilityGroups")]
        public async Task<IActionResult> AddFacilityGroups([FromBody] List<AddGroupDTO> dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (dto is null)
                    return NoContent();

                foreach (AddGroupDTO group in dto)
                {
                    if (group.Id is null || group.Id == 0)
                        return NoContent();
                    if (String.IsNullOrWhiteSpace(group.Name))
                        return NoContent();

                    if(group.AddGroupKey is [_, ..])
                    {
                        foreach(var key in group.AddGroupKey)
                        {
                            if (String.IsNullOrWhiteSpace(key.Name))
                                return NoContent();

                            if (key.ItemValues is [_, ..])
                            {
                                foreach(var value in key.ItemValues)
                                {
                                    if (String.IsNullOrWhiteSpace(value.Values))
                                        return NoContent();
                                }
                            }
                        }
                    }
                }

                ResponseModel<bool> model = await GroupService.AddFacilityGroupKeyValueService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
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
        [HttpGet]
        [Route("sign/GetFacilityGroup")]
        public async Task<IActionResult> GetDetailFacility([FromQuery][Required]int Facilityid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<GroupListDTO>> model = await GroupService.GetFacilityGroupListService(Facilityid).ConfigureAwait(false);
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

        /// <summary>
        /// 그룹명 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async Task<IActionResult> UpdateFacilityGroup([FromBody] UpdateGroupDTO dto)
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

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async Task<IActionResult> DeleteFacilityGroup([FromBody][Required]int groupid)
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
