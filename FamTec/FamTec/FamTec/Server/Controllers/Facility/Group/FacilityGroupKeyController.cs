using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Facility.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupKeyController : ControllerBase
    {
        private readonly IFacilityKeyService FacilityKeyService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<FacilityGroupKeyController> CreateBuilderLogger; /* 콘솔로그 */

        public FacilityGroupKeyController(IFacilityKeyService _facilitykeyservice,
            ILogService _logservice,
            ConsoleLogService<FacilityGroupKeyController> _createbuilderlogger)
        {
            this.FacilityKeyService = _facilitykeyservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async Task<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<AddKeyDTO> model = await FacilityKeyService.AddKeyService(dto).ConfigureAwait(false);

                if (dto.GroupID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.ItemValues is [_, ..])
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Itemkey))
                    return NoContent();

                ResponseModel<UpdateKeyDTO> model = await FacilityKeyService.UpdateKeyService(dto).ConfigureAwait(false);

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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteKeyList")]
        public async Task<IActionResult> DeleteGroupKeyList([FromBody][Required]List<int> keylist)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (keylist is null)
                    return NoContent();
                
                if(keylist.Count() == 0)
                    return NoContent();

                ResponseModel<bool?> model = await FacilityKeyService.DeletKeyListService(keylist).ConfigureAwait(false);
                
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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async Task<IActionResult> DeleteGroupKey([FromBody][Required]int keyid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<bool?> model = await FacilityKeyService.DeleteKeyService(keyid).ConfigureAwait(false);

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
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
