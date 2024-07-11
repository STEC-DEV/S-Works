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

        public BuildingGroupController(IBuildingGroupService _groupservice)
        {
            this.GroupService = _groupservice;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBuildingGroup")]
        public async ValueTask<IActionResult> AddBuildingGroup([FromBody] AddGroupDTO? dto)
        {
            //AddGroupDTO dto = new AddGroupDTO();
            //dto.BuildingIdx = 30;
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

            ResponseUnit<AddGroupDTO?> model = await GroupService.AddBuildingGroupService(HttpContext, dto);
            
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
        [HttpGet]
        [Route("sign/GetBuildingGroup")]
        public async ValueTask<IActionResult> GetDetailBuilding(int? buildingid)
        {
            ResponseList<GroupListDTO?> model = await GroupService.GetBuildingGroupListService(HttpContext, buildingid);
            if (model is not null)
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
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async ValueTask<IActionResult> UpdateBuildingGroup([FromBody] UpdateGroupDTO? dto)
        {
            ResponseUnit<bool?> model = await GroupService.UpdateGroupNameService(HttpContext, dto);
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
        [Route("sign/DeleteGroup")]
        public async ValueTask<IActionResult> DeleteBuildingGroup(int? groupid)
        {
            ResponseUnit<bool?> model = await GroupService.DeleteGroupService(HttpContext, groupid);
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


    }
}
