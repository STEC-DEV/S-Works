﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Meter.Contract;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Meter.Contract
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService ContractService;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<ContractController> CreateBuilderLogger;

        public ContractController(IContractService _contractservice,
            ILogService _logservice,
            ConsoleLogService<ContractController> _createbuilderlogger)
        {
            this.ContractService = _contractservice;

            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.data?.ToString() ?? ""} --> {HttpContext.Request.Path.Value}");
#endif

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.data?.ToString() ?? ""} --> {HttpContext.Request.Path.Value}");
#endif

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
