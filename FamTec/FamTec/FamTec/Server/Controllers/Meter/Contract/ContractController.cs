using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter.Contract;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter.Contract
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private IContractService ContractService;
        private ILogService LogService;


        public ContractController(IContractService _contractservice, ILogService _logservice)
        {
            this.ContractService = _contractservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 계약종류 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddContract")]
        public async Task<IActionResult> AddContract([FromBody]AddContractDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddContractDTO> model = await ContractService.AddContractService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
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
        /// 계약종류 전체조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllContract")]
        public async Task<IActionResult> GetAllContract()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ContractDTO>? model = await ContractService.GetAllContractListService(HttpContext);
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
