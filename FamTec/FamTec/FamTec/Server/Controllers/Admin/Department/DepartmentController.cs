﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Department;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Admin.Department
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService DepartmentService;
        private readonly ILogService LogService;

        // 콘솔로그
        private readonly ConsoleLogService<DepartmentController> CreateBuilderLogger;

        public DepartmentController(IDepartmentService _departmentservice,
            ILogService _logservice,
            ConsoleLogService<DepartmentController> _createbuilderlogger)
        {
            this.DepartmentService = _departmentservice;
            
            this.LogService = _logservice;
            // 콘솔로그
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 부서추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] AddDepartmentDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.ManagerYN is null)
                    return NoContent();

                ResponseUnit<AddDepartmentDTO>? model = await DepartmentService.AddDepartmentService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 202) // 이미 해당 이름으로 부서가 존재함.
                    return Ok(model);
                else
                    return BadRequest(model);
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
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetDepartmentList")]
        public async Task<IActionResult> GetAllDepartment()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DepartmentDTO>? model = await DepartmentService.GetAllDepartmentService().ConfigureAwait(false);
                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
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

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetManageDepartmentList")]
        public async Task<IActionResult> GetManageDepartmentList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DepartmentDTO>? model = await DepartmentService.ManageDepartmentService().ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
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
        /// 부서삭제
        /// </summary>
        /// <param name="selList"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartmentList([FromBody]List<int> departmentidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await DepartmentService.DeleteDepartmentService(HttpContext, departmentidx).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 400)
                    return Ok(model);
                else
                    return BadRequest(model);
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
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.Id is null)
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.ManagerYN is null)
                    return NoContent();

                ResponseUnit<DepartmentDTO>? model = await DepartmentService.UpdateDepartmentService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
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
