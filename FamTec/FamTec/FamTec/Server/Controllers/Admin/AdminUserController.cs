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
        private IFileService FileService;
        private ILogService LogService;

        public AdminUserController(IAdminAccountService _adminservice,
            IAdminPlaceService _adminplaceservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.AdminAccountService = _adminservice;
            this.AdminPlaceService = _adminplaceservice;
            this.FileService = _fileservice;

            this.LogService = _logservice;
        }


        /// <summary>
        /// 관리자아이디 생성
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager,Master")]
        [HttpPost]
        //[HttpGet]
        [Route("sign/AddManager")]
        //public async Task<IActionResult> AddManager( [FromForm] IFormFile? files)
        public async Task<IActionResult> AddManager([FromForm] AddManagerDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                //AddManagerDTO dto = new AddManagerDTO();
                //dto.UserId = "MASTER0005";
                //dto.Password = "123";
                //dto.Name = "테스트이름";
                //dto.Phone = "0123";
                //dto.DepartmentId = 5;

                if (HttpContext is null)
                    return BadRequest();
                
                // * ID 필수
                if (String.IsNullOrWhiteSpace(dto.UserId)) 
                    return NoContent();
                
                // * 비밀번호 필수
                if (String.IsNullOrWhiteSpace(dto.Password)) 
                    return NoContent();
                
                // * 부서필수
                if (dto.DepartmentId is null)
                    return NoContent();

                if (files is not null)
                {
                    string? extension = FileService.GetExtension(files);
                    if(String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<int?> model = await AdminAccountService.AdminRegisterService(HttpContext, dto, files).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        public async Task<IActionResult> GetManagerInfo([FromQuery]int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DManagerDTO>? model = await AdminAccountService.DetailAdminService(adminid).ConfigureAwait(false);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        public async Task<IActionResult> AddManagerWorks([FromBody] AddManagerPlaceDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.AddManagerPlaceSerivce(HttpContext, dto).ConfigureAwait(false);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        public async Task<IActionResult> DeleteManager([FromBody]List<int> adminidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (adminidx is null)
                    return NoContent();
                
                if (adminidx.Count == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await AdminAccountService.DeleteAdminService(HttpContext, adminidx).ConfigureAwait(false);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


        /// <summary>
        /// 관리자 이미지 변경
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateManagerImage")]
        public async Task<IActionResult> UpdateManagerImage([FromForm]int adminid, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (adminid is 0)
                    return NoContent();

                if (files is not null)
                {
                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await AdminAccountService.UpdateAdminImageService(HttpContext, adminid, files).ConfigureAwait(false);
                
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


        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        //[HttpGet]
        [Route("sign/UpdateManager")]
        //public async Task<IActionResult> UpdateManager()

        public async Task<IActionResult> UpdateManager([FromBody] UpdateManagerDTO dto)
        {
            try
            {
                
                //UpdateManagerDTO dto = new UpdateManagerDTO();
                //dto.AdminIndex = 10;
                //dto.Name = "용";
                //dto.DepartmentId = 5;
                //dto.UserId = "Master";
                //dto.Password = "123";
                //dto.PlaceList.Add(new AdminPlaceDTO
                //{
                //    Id = 3
                //});
                //dto.PlaceList.Add(new AdminPlaceDTO
                //{
                //    Id = 11
                //});
               

                if (HttpContext is null)
                    return BadRequest();

                if (dto.AdminIndex is null)
                    return NoContent();
                
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();
                
                if(dto.DepartmentId is null)
                    return NoContent();
                
                if(String.IsNullOrWhiteSpace(dto.UserId))
                    return NoContent();
                
                if (String.IsNullOrWhiteSpace(dto.Password))
                    return NoContent();

                ResponseUnit<bool?> model = await AdminAccountService.UpdateAdminService(HttpContext, dto).ConfigureAwait(false);
                
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

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/UserIdCheck")]
        public async Task<IActionResult> UserIdCheck([FromBody] string userid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminAccountService.UserIdCheckService(userid).ConfigureAwait(false);
                
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
