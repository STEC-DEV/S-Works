using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupKeyController : ControllerBase
    {
        private readonly IBuildingKeyService BuildingKeyService;
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<BuildingGroupKeyController> CreateBuilderLogger; /* 콘솔로그 */

        public BuildingGroupKeyController(IBuildingKeyService _buildingkeyservice,
            ILogService _logservice,
            ConsoleLogService<BuildingGroupKeyController> _createbuilderlogger)
        {
            this.BuildingKeyService = _buildingkeyservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// Key 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

                ResponseModel<AddKeyDTO> model = await BuildingKeyService.AddKeyService(dto).ConfigureAwait(false);

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

        // 수정 - 삭제 확인

        // 수정
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

                ResponseModel<UpdateKeyDTO> model = await BuildingKeyService.UpdateKeyService(dto).ConfigureAwait(false);
                
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
        public async Task<IActionResult> DeleteGroupKeyList([FromQuery]List<int> keylist)
        {
            try
            {

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (keylist is null)
                    return NoContent();
                if (keylist.Count() == 0)
                    return NoContent();

                ResponseModel<bool?> model = await BuildingKeyService.DeleteKeyListService(keylist).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 키 삭제
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async Task<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            try
            {

#if DEBUG
                CreateBuilderLogger.ConsoleText($" {HttpContext.Request.Path.Value}");
#endif

                ResponseModel<bool?> model = await BuildingKeyService.DeleteKeyService(keyid).ConfigureAwait(false);

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
