using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupController : ControllerBase
    {
        private readonly IBuildingGroupService GroupService;
        private readonly ILogService LogService;
        private readonly ILogger<BuildingGroupController> BuilderLogger;

        public BuildingGroupController(IBuildingGroupService _groupservice,
            ILogService _logservice,
            ILogger<BuildingGroupController> _builderlogger)
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

                if (dto.BuildingIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupInfoDTO> model = await GroupService.AddBuildingGroupInfoService(HttpContext, dto).ConfigureAwait(false);
                
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
        //[HttpGet]
        [HttpPost]
        [Route("sign/AddBuildingGroup")]
        //public async Task<IActionResult> AddBuildingGroup()
        public async Task<IActionResult> AddBuildingGroup([FromBody] List<AddGroupDTO> dto)
        {
            try
            {
                // ------------- DTO 검사
                if (dto is null)
                    return NoContent();

                foreach(AddGroupDTO group in dto)
                {
                    if(group.BuildingIdx is null || group.BuildingIdx == 0)
                        return NoContent();
                    if(String.IsNullOrWhiteSpace(group.Name))
                        return NoContent();

                    if (group.AddGroupKey is [_, ..])
                    {
                        foreach (var key in group.AddGroupKey)
                        {
                            if (String.IsNullOrWhiteSpace(key.Name))
                                return NoContent();
                            //if (String.IsNullOrWhiteSpace(key.Unit))
                            //    return NoContent();

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


                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool> model = await GroupService.AddBuildingGroupService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

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
                CreateBuilderLogger(ex);
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
        public async Task<IActionResult> GetDetailBuilding(int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<GroupListDTO?> model = await GroupService.GetBuildingGroupListService(HttpContext, buildingid).ConfigureAwait(false);
                
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
                CreateBuilderLogger(ex);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async Task<IActionResult> DeleteBuildingGroup([FromBody]int groupid)
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
