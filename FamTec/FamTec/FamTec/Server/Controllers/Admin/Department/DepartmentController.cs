using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Department;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Admin.Department
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private IDepartmentService DepartmentService;
        private ILogService LogService;

        public DepartmentController(IDepartmentService _departmentservice,
            ILogService _logservice)
        {
            this.DepartmentService = _departmentservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 부서추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddDepartment")]
        public async ValueTask<IActionResult> AddDepartment([FromBody] AddDepartmentDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddDepartmentDTO>? model = await DepartmentService.AddDepartmentService(HttpContext, dto);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetDepartmentList")]
        public async ValueTask<IActionResult> GetAllDepartment()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<DepartmentDTO>? model = await DepartmentService.GetAllDepartmentService();
                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> DeleteDepartmentList([FromBody]List<int> departmentidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool>? model = await DepartmentService.DeleteDepartmentService(HttpContext, departmentidx);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> UpdateDepartment([FromBody] DepartmentDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DepartmentDTO>? model = await DepartmentService.UpdateDepartmentService(HttpContext, dto);

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
