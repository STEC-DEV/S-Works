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
        [Route("sign/AddGroup")]
        public async ValueTask<IActionResult> AddGroup([FromBody] AddGroupInfoDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.FacilityIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupInfoDTO> model = await GroupService.AddFacilityGroupInfoService(HttpContext, dto);
                
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
        [Route("sign/AddFacilityGroup")]
        public async ValueTask<IActionResult> AddFacilityGroup([FromBody] AddGroupDTO dto)
        {
            try
            {
                //AddGroupDTO dto = new AddGroupDTO();
                //dto.FacilityIdx = 4;
                //dto.Name = "송풍기";

                //dto.AddGroupKey.Add(new AddGroupItemKeyDTO()
                //{
                //    Name = "모터용량",
                //    Unit = "RPM",
                //    ItemValues = new List<AddGroupItemValueDTO>()
                //    {
                //        new AddGroupItemValueDTO()
                //        {
                //            Values = "15KW x 3EA"
                //        },
                //        new AddGroupItemValueDTO()
                //        {
                //            Values = "20KW x 2EA"
                //        }
                //    }
                //});
                

                if (HttpContext is null)
                    return BadRequest();

                if (dto.FacilityIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupDTO> model = await GroupService.AddFacilityGroupService(HttpContext, dto);

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

                ResponseList<GroupListDTO> model = await GroupService.GetFacilityGroupListService(HttpContext, Facilityid);
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
        /// 그룹명 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async ValueTask<IActionResult> UpdateFacilityGroup([FromBody] UpdateGroupDTO dto)
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

        /// <summary>
        /// 그룹 삭제
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DeleteGroup")]
        public async ValueTask<IActionResult> DeleteFacilityGroup([FromQuery]int groupid)
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
