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

        public DepartmentController(IDepartmentService _departmentservice)
        {
            this.DepartmentService = _departmentservice;
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
            ResponseUnit<AddDepartmentDTO>? model = await DepartmentService.AddDepartmentService(HttpContext, dto);

            if(model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
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
            ResponseList<DepartmentDTO>? model = await DepartmentService.GetAllDepartmentService();
            if(model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
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
            ResponseUnit<bool>? model = await DepartmentService.DeleteDepartmentService(HttpContext, departmentidx);

            if(model is not null)
            {
                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            else
            {
                return BadRequest(model);
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
            ResponseUnit<DepartmentDTO>? model = await DepartmentService.UpdateDepartmentService(HttpContext, dto);

            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
