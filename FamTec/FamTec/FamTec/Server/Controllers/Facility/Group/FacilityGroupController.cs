using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupController : ControllerBase
    {
        private IFacilityGroupService GroupService;
        private ILogService LogService;


        public FacilityGroupController(IFacilityGroupService _groupservice,
            ILogService _logservice)
        {
            this.GroupService = _groupservice;
            this.LogService = _logservice;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddFacilityGroup")]
        public async ValueTask<IActionResult> AddFacilityGroup([FromBody] AddGroupDTO dto)
        {
            try
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

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddGroupDTO?> model = await GroupService.AddFacilityGroupService(HttpContext, dto);

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
        [Route("sign/GetFacilityGroup")]
        public async ValueTask<IActionResult> GetDetailFacility(int Facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<GroupListDTO?> model = await GroupService.GetFacilityGroupListService(HttpContext, Facilityid);
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
        [Route("sign/UpdateGroup")]
        public async ValueTask<IActionResult> UpdateFacilityGroup([FromBody] UpdateGroupDTO dto)
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
        public async ValueTask<IActionResult> DeleteFacilityGroup([FromBody]int groupid)
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
