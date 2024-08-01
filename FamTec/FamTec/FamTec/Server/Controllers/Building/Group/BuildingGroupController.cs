using FamTec.Server.Services;
using FamTec.Server.Services.Building.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [Route("api/[controller]")]
    [ApiController]
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

                ResponseUnit<AddGroupInfoDTO?> model = await GroupService.AddBuildingGroupInfoService(HttpContext, dto);
                
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
                //dto.BuildingIdx = 6;
                //dto.Name = "옥상주차장";

                //AddGroupItemKeyDTO key = new AddGroupItemKeyDTO();
                //key.Name = "전기차";

                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "3",
                //    Unit = "대"
                //});

                //dto.AddGroupKey.Add(key);

                //key = new AddGroupItemKeyDTO();
                //key.Name = "수소차";

                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "5",
                //    Unit = "대"
                //});
                //key.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "8",
                //    Unit = "대"
                //});
                //dto.AddGroupKey.Add(key);

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddGroupDTO?> model = await GroupService.AddBuildingGroupService(HttpContext, dto);

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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async ValueTask<IActionResult> UpdateBuildingGroup([FromBody] UpdateGroupDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

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
