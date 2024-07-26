using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupKeyController : ControllerBase
    {
        private IFacilityKeyService FacilityKeyService;
        private ILogService LogService;

        public FacilityGroupKeyController(IFacilityKeyService _facilitykeyservice,
            ILogService _logservice)
        {
            this.FacilityKeyService = _facilitykeyservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async ValueTask<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            try
            {
                //AddKeyDTO dto = new AddKeyDTO();
                //dto.GroupID = 1;
                //dto.Name = "부하축베어링";
                //dto.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "6309ZZ"
                //});
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddKeyDTO?> model = await FacilityKeyService.AddKeyService(HttpContext, dto);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateKey")]
        public async ValueTask<IActionResult> UpdateGroupKey([FromBody]UpdateKeyDTO dto)
        {
            try
            {
                //UpdateKeyDTO dto = new UpdateKeyDTO();
                //dto.ID = 3;
                //dto.Itemkey = "수정_부하축베어링";
                //dto.ValueList.Add(new GroupValueListDTO()
                //{
                //    ID = 3,
                //    ItemValue = "6333XX"
                //});

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UpdateKeyDTO?> model = await FacilityKeyService.UpdateKeyService(HttpContext, dto);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async ValueTask<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await FacilityKeyService.DeleteKeyService(HttpContext, keyid);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }



    }
}
