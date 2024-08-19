using FamTec.Server.Services;
using FamTec.Server.Services.BlackList;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.BlackList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.BlackList
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlackListController : ControllerBase
    {
        private IBlackListService BlackListService;
        private ILogService LogService;

        public BlackListController(IBlackListService _blacklistservice, 
            ILogService _logservice)
        {
            this.BlackListService = _blacklistservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 블랙리스트 번호 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBlackList")]
        public async ValueTask<IActionResult> AddBlackList(AddBlackListDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return NoContent();

                ResponseUnit<AddBlackListDTO> model = await BlackListService.AddBlackList(HttpContext, dto);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 블랙리스트 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllBlackList")]
        public async ValueTask<IActionResult> GetAllBlackList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<BlackListDTO> model = await BlackListService.GetAllBlackList(HttpContext);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBlackList")]
        public async ValueTask<IActionResult> UpdateBlackList(BlackListDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return NoContent();

                ResponseUnit<bool?> model = await BlackListService.UpdateBlackList(HttpContext, dto);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteBlackList")]
        public async ValueTask<IActionResult> DeleteBlackList(List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BlackListService.DeleteBlackList(HttpContext, delIdx);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
