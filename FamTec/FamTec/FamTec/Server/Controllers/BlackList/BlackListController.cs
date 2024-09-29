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
        public async Task<IActionResult> AddBlackList([FromBody]AddBlackListDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return NoContent();

                ResponseUnit<AddBlackListDTO> model = await BlackListService.AddBlackList(HttpContext, dto).ConfigureAwait(false);
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
        public async Task<IActionResult> GetAllBlackList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<BlackListDTO> model = await BlackListService.GetAllBlackList(HttpContext).ConfigureAwait(false);
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
        /// 블랙리스트 카운트 출력
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetBlackListCount")]
        public async Task<IActionResult> GetAllBlackListCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await BlackListService.GetBlackListCountService(HttpContext).ConfigureAwait(false);
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
        /// 블랙리스트 페이지네이션 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllPageNationBlackList")]
        public async Task<IActionResult> GetAllBlackList([FromQuery]int pagenum, [FromQuery]int pagesize)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (pagesize > 100)
                    return BadRequest(); // 사이즈 초과

                if (pagenum == 0)
                    return BadRequest(); // 잘못된 요청

                if (pagesize == 0)
                    return BadRequest(); // 잘못된 요청

                ResponseList<BlackListDTO> model = await BlackListService.GetAllBlackListPageNation(HttpContext, pagenum, pagesize).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
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
        public async Task<IActionResult> UpdateBlackList(BlackListDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.PhoneNumber))
                    return NoContent();

                ResponseUnit<bool?> model = await BlackListService.UpdateBlackList(HttpContext, dto).ConfigureAwait(false);
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
        public async Task<IActionResult> DeleteBlackList(List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BlackListService.DeleteBlackList(HttpContext, delIdx).ConfigureAwait(false);
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