using FamTec.Server.Services.Admin.Account;
using FamTec.Server.Services.Admin.Place;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private IAdminAccountService AdminAccountService;
        private IAdminPlaceService AdminPlaceService;

        public AdminUserController(IAdminAccountService _adminservice,
            IAdminPlaceService _adminplaceservice)
        {
            this.AdminAccountService = _adminservice;
            this.AdminPlaceService = _adminplaceservice;
        }


        /// <summary>
        /// 관리자아이디 생성
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager,Master")]
        [HttpPost]
        [Route("sign/AddManager")]
        public async ValueTask<IActionResult> AddManager([FromForm] AddManagerDTO dto, [FromForm]IFormFile? files)
        {
            ResponseUnit<int?> model = await AdminAccountService.AdminRegisterService(HttpContext, dto, files);

            if (model is not null)
            {
                if (model.code == 200)
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

        /// <summary>
        /// 매니저 상세보기 페이지
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/DetailManagerInfo")]
        public async ValueTask<IActionResult> GetManagerInfo([FromQuery]int adminid)
        {
            ResponseUnit<DManagerDTO>? model = await AdminAccountService.DetailAdminService(adminid);

            if(model is not null)
            {
                if(model.code == 200)
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
        /// 관리자추가시 사업장등록 [수정완료]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddManagerWorks")]
        public async ValueTask<IActionResult> AddManagerWorks([FromBody] AddManagerPlaceDTO dto)
        {
            ResponseUnit<bool>? model = await AdminPlaceService.AddManagerPlaceSerivce(HttpContext, dto);

            if(model is not null)
            {
                if(model.code == 200)
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
        /// 관리자 삭제 - 유저테이블 - 사업장테이블 모두 삭제
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeleteManager")]
        public async ValueTask<IActionResult> DeleteManager(List<int> adminidx)
        {
            ResponseUnit<int?> model = await AdminAccountService.DeleteAdminService(HttpContext, adminidx);

            if(model is not null)
            {
                if(model.code == 200)
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
        /// 관리자 정보 수정
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateManager")]
        public async ValueTask<IActionResult> UpdateManager([FromBody] UpdateManagerDTO? dto, IFormFile? files)
        {
            ResponseUnit<int?> model = await AdminAccountService.UpdateAdminService(HttpContext, dto, files);
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

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/UserIdCheck")]
        public async ValueTask<IActionResult> UserIdCheck([FromBody] string userid)
        {
            ResponseUnit<bool?> model = await AdminAccountService.UserIdCheckService(userid);
            if (model is not null)
            {
                if (model.code == 200)
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
