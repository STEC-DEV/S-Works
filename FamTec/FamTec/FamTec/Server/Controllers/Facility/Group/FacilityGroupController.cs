using FamTec.Server.Services.Facility.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupController : ControllerBase
    {
        private IFacilityGroupService GroupService;

        public FacilityGroupController(IFacilityGroupService _groupservice)
        {
            this.GroupService = _groupservice;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFacilityGroup")]
        public async ValueTask<IActionResult> AddFacilityGroup([FromBody] AddGroupDTO? dto)
        {
            //AddGroupDTO dto = new AddGroupDTO();
            //dto.FacilityIdx = 2;
            //dto.Name = "송풍기";

            //AddGroupItemKeyDTO key = new AddGroupItemKeyDTO();
            //key.Name = "모터용량";

            //key.ItemValues.Add(new AddGroupItemValueDTO()
            //{
            //    Values = "15KW x 2EA"
            //});
            //dto.AddGroupKey.Add(key);

            //key = new AddGroupItemKeyDTO();
            //key.Name = "정압";
            //key.ItemValues.Add(new AddGroupItemValueDTO()
            //{
            //    Values = "120",
            //    Unit = "mmAq"
            //});
            //dto.AddGroupKey.Add(key);

            ResponseUnit<AddGroupDTO?> model = await GroupService.AddFacilityGroupService(HttpContext, dto);
            
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
        [HttpGet]
        [Route("sign/GetFacilityGroup")]
        public async ValueTask<IActionResult> GetDetailFacility(int? Facilityid)
        {
            ResponseList<GroupListDTO?> model = await GroupService.GetFacilityGroupListService(HttpContext, Facilityid);
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
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async ValueTask<IActionResult> UpdateFacilityGroup([FromBody] UpdateGroupDTO? dto)
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
        public async ValueTask<IActionResult> DeleteFacilityGroup([FromBody]int? groupid)
        {
            ResponseUnit<bool?> model = await GroupService.DeleteGroupService(HttpContext, groupid);
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
