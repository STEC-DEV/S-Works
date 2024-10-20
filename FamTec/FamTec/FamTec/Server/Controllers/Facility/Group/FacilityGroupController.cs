using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupController : ControllerBase
    {
        private readonly IFacilityGroupService GroupService;
        private readonly ILogService LogService;
        private readonly ILogger<FacilityGroupController> BuilderLogger;

        public FacilityGroupController(IFacilityGroupService _groupservice,
            ILogService _logservice,
            ILogger<FacilityGroupController> _builderlogger)
        {
            this.GroupService = _groupservice;
            
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
        [Route("sign/AddGroup")]
        public async Task<IActionResult> AddGroup([FromBody] AddGroupInfoDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.FacilityIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupInfoDTO> model = await GroupService.AddFacilityGroupInfoService(HttpContext, dto).ConfigureAwait(false);
                
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
                if (HttpContext is null)
                    return BadRequest();

                if (dto.Id is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupDTO> model = await GroupService.AddFacilityGroupService(HttpContext, dto).ConfigureAwait(false);

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
        [Route("sign/AddFacilityGroups")]
        public async Task<IActionResult> AddFacilityGroups([FromBody] List<AddGroupDTO> dto)
        {
            try
            {
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

                            if(key.ItemValues is [_, ..])
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

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool> model = await GroupService.AddFacilityGroupKeyValueService(HttpContext, dto).ConfigureAwait(false);

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
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetFacilityGroup")]
        public async Task<IActionResult> GetDetailFacility([FromQuery]int Facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<GroupListDTO> model = await GroupService.GetFacilityGroupListService(HttpContext, Facilityid).ConfigureAwait(false);
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
                if (HttpContext is null)
                    return BadRequest();

                if (dto.GroupId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.GroupName))
                    return NoContent();


                ResponseUnit<bool?> model = await GroupService.UpdateGroupNameService(HttpContext, dto).ConfigureAwait(false);
                
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

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async Task<IActionResult> DeleteFacilityGroup([FromBody]int groupid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await GroupService.DeleteGroupService(HttpContext, groupid).ConfigureAwait(false);
                
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
