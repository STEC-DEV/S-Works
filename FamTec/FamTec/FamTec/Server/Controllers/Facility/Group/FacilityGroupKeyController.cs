using DocumentFormat.OpenXml.Bibliography;
using FamTec.Client.Pages.Admin.Manager.ManagerDetail.Components;
using FamTec.Server.Middleware;
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
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
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
        public async Task<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            try
            {
                //AddKeyDTO dto = new AddKeyDTO();
                //dto.GroupID = 1;
                //dto.Name = "부하축베어링";
                //dto.Unit = "단위1";
                //dto.ItemValues.Add(new AddGroupItemValueDTO()
                //{
                //    Values = "6309ZZ"
                //});

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddKeyDTO> model = await FacilityKeyService.AddKeyService(HttpContext, dto).ConfigureAwait(false);

                if (dto.GroupID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                if(dto.ItemValues is [_, ..])
                {
                    foreach(AddGroupItemValueDTO ValueDTO in dto.ItemValues)
                    {
                        if (String.IsNullOrWhiteSpace(ValueDTO.Values))
                            return NoContent();
                    }
                }

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
        [Route("sign/UpdateKey")]
        public async Task<IActionResult> UpdateGroupKey([FromBody]UpdateKeyDTO dto)
        {
            try
            {
                //UpdateKeyDTO dto = new UpdateKeyDTO();
                //dto.ID = 8;
                //dto.Itemkey = "수정_부하축베어링";
                //dto.Unit = "수정단위1";

                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Itemkey))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                ResponseUnit<UpdateKeyDTO> model = await FacilityKeyService.UpdateKeyService(HttpContext, dto).ConfigureAwait(false);

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
        [Route("sign/DeleteKeyList")]
        public async Task<IActionResult> DeleteGroupKeyList([FromBody]List<int> keylist)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (keylist is null)
                    return NoContent();
                
                if(keylist.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await FacilityKeyService.DeletKeyListService(HttpContext, keylist).ConfigureAwait(false);
                
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
        [Route("sign/DeleteKey")]
        public async Task<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await FacilityKeyService.DeleteKeyService(HttpContext, keyid).ConfigureAwait(false);

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
