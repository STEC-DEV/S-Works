using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Group;
using FamTec.Shared.Client.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class BuildingGroupController : ControllerBase
    {
        private IBuildingGroupService GroupService;
        private ILogService LogService;

        public BuildingGroupController(IBuildingGroupService _groupservice,
            ILogService _logservice)
        {
            this.GroupService = _groupservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddGroup")]
        public async ValueTask<IActionResult> AddGroup([FromBody] AddGroupInfoDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.BuildingIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupInfoDTO> model = await GroupService.AddBuildingGroupInfoService(HttpContext, dto);
                
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
        [Route("sign/AddBuildingGroup")]
        public async ValueTask<IActionResult> AddBuildingGroup([FromBody] AddGroupDTO dto)
        {
            try
            {
                //AddGroupDTO dto = new AddGroupDTO();
                //dto.BuildingIdx = 12;
                //dto.Name = "옥상주차장";

                //AddGroupItemKeyDTO key = new AddGroupItemKeyDTO();
                //key.Name = "전기차";
                //key.Unit = "대";

                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "3",
                //});

                //dto.AddGroupKey.Add(key);

                //key = new AddGroupItemKeyDTO();
                //key.Name = "수소차";
                //key.Unit = "대";

                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "5"
                //});
                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "8"
                //});

                //dto.AddGroupKey.Add(key);

                // ------------- DTO 검사
                if (dto is null)
                    return NoContent();

                if(dto.BuildingIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.AddGroupKey is null)
                    return NoContent();
                if(dto.AddGroupKey.Count == 0)
                    return NoContent();

                foreach (AddGroupItemKeyDTO KeyDTO in dto.AddGroupKey)
                {
                    if (String.IsNullOrWhiteSpace(KeyDTO.Name))
                        return NoContent();
                    if (String.IsNullOrWhiteSpace(KeyDTO.Unit))
                        return NoContent();

                    if(KeyDTO.ItemValues is null)
                        return NoContent();
                    if(KeyDTO.ItemValues.Count == 0)
                        return NoContent();

                    foreach (AddGroupItemValueDTO ValueDTO in KeyDTO.ItemValues)
                    {
                        if(String.IsNullOrWhiteSpace(ValueDTO.Values))
                            return NoContent();
                    }
                    
                }

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddGroupDTO> model = await GroupService.AddBuildingGroupService(HttpContext, dto);

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

        /// <summary>
        /// 건물의 하위정보 전체조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetBuildingGroup")]
        public async ValueTask<IActionResult> GetDetailBuilding(int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<GroupListDTO?> model = await GroupService.GetBuildingGroupListService(HttpContext, buildingid);
                
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
        public async ValueTask<IActionResult> UpdateBuildingGroup([FromBody] UpdateGroupDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.GroupId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.GroupName))
                    return NoContent();

                ResponseUnit<bool?> model = await GroupService.UpdateGroupNameService(HttpContext, dto);
                
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
        //[HttpGet]
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async ValueTask<IActionResult> DeleteBuildingGroup([FromBody]int groupid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                ResponseUnit<bool?> model = await GroupService.DeleteGroupService(HttpContext, groupid);
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
