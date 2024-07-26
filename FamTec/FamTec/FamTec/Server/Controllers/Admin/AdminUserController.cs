using FamTec.Server.Services.Admin.Account;
using FamTec.Server.Services.Admin.Place;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private IAdminAccountService AdminAccountService;
        private IAdminPlaceService AdminPlaceService;
        private ILogService LogService;

        public AdminUserController(IAdminAccountService _adminservice,
            IAdminPlaceService _adminplaceservice,
            ILogService _logservice)
        {
            this.AdminAccountService = _adminservice;
            this.AdminPlaceService = _adminplaceservice;
            this.LogService = _logservice;
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
            try
            {
                //var file = Request.Form.Files[0];
                //if (file.Length > 0)
                //{
                //    // 파일명 생성 (중복 방지를 위해 GUID 사용)
                //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                //    var filePath = Path.Combine(@"X:\\FMS\\설계\\ERD", fileName);

                //    // 파일 저장
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await file.CopyToAsync(stream);
                //    }

                //    // 파일 경로를 데이터베이스에 저장하거나 필요한 처리를 수행
                //    Console.WriteLine(filePath);
                //}

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminAccountService.AdminRegisterService(HttpContext, dto, files);

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
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DManagerDTO>? model = await AdminAccountService.DetailAdminService(adminid);

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
        /// 관리자추가시 사업장등록 [수정완료]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddManagerWorks")]
        public async ValueTask<IActionResult> AddManagerWorks([FromBody] AddManagerPlaceDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool>? model = await AdminPlaceService.AddManagerPlaceSerivce(HttpContext, dto);

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
        /// 관리자 삭제 - 유저테이블 - 사업장테이블 모두 삭제
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeleteManager")]
        public async ValueTask<IActionResult> DeleteManager(List<int> adminidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminAccountService.DeleteAdminService(HttpContext, adminidx);

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
        /// 관리자 정보 수정
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateManager")]
        public async ValueTask<IActionResult> UpdateManager([FromBody] UpdateManagerDTO? dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminAccountService.UpdateAdminService(HttpContext, dto, files);
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
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminAccountService.UserIdCheckService(userid);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }


    }
}
