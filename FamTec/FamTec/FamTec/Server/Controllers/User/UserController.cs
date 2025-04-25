using Microsoft.AspNetCore.Mvc;
using FamTec.Shared.Server.DTO.User;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using Microsoft.AspNetCore.Authorization;
using FamTec.Server.Services;
using FamTec.Server.Middleware;
using FamTec.Server.Services.Admin.Account;
using System.ComponentModel.DataAnnotations;
using FamTec.Server.Helpers;

namespace FamTec.Server.Controllers.User
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAdminAccountService AdminAccountService;
        private readonly IUserService UserService;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ICommService CommService; /* 핼퍼 클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<UserController> CreateBuilderLogger; /* 콘솔로그 */

        public UserController(IUserService _userservice,
            IAdminAccountService _adminservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<UserController> _createbuilderlogger)
        {
            this.UserService = _userservice;
            this.AdminAccountService = _adminservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 일반 화면 가이드 PDF 다운로드
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DownloadUserGuideForm")]
        public async Task<IActionResult> DownloadUserGuideForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                byte[]? fileBytes = await UserService.DownloadUserGuidForm().ConfigureAwait(false);

                if (fileBytes is not null)
                    return File(fileBytes, "application/pdf", "S-Works_사용자설명서_1.3_KO_241211.pdf");
                else
                    return Ok();
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

        [HttpGet]
        [Route("DownloadUserForm")]
        public async Task<IActionResult> DownloadUserForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                byte[]? fileBytes = await UserService.DownloadUserForm().ConfigureAwait(false);

                if (fileBytes is not null)
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "사용자정보.xlsx");
                else
                    return Ok();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportUser")]
        public async Task<IActionResult> ImportUserData([FromForm][Required] IFormFile files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (files is null)
                    return NoContent();

                if (files.Length == 0)
                    return NoContent();

                string? extension = FileService.GetExtension(files); // 파일 확장자 추출
                if (String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension); // 파일 확장자 검사
                    if(!extensioncheck)
                    {
                        return Ok(new ResponseModel<bool>() { message = "지원하지 않는 파일형식입니다.", data = false, code = 204 });
                    }
                }

                ResponseModel<bool> model = await UserService.ImportUserService(files).ConfigureAwait(false);
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
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


        /// <summary>
        /// 로그인한 사업장의 모든 사용자 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceUsers")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<ListUser>>? model = await UserService.GetPlaceUserList().ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사용자 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUser")]
        public async Task<IActionResult> AddUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (String.IsNullOrWhiteSpace(dto.USERID)) return NoContent(); // 사용자ID
                if(String.IsNullOrWhiteSpace(dto.PASSWORD)) return NoContent(); // 사용자 비밀번호
                if(dto.PERM_BASIC is null) return NoContent(); // 기본정보메뉴 권한
                if(dto.PERM_MACHINE is null) return NoContent(); // 기계메뉴 권한
                if(dto.PERM_ELEC is null) return NoContent(); // 전기메뉴 권한
                if(dto.PERM_LIFT is null) return NoContent(); // 승강메뉴 권한
                if(dto.PERM_FIRE is null) return NoContent(); // 소방메뉴 권한
                if(dto.PERM_CONSTRUCT is null) return NoContent(); // 건축메뉴 권한
                if(dto.PERM_NETWORK is null) return NoContent(); // 통신메뉴 권한
                if(dto.PERM_BEAUTY is null) return NoContent(); // 미화메뉴 권한
                if(dto.PERM_SECURITY is null) return NoContent(); // 보안메뉴 권한
                if(dto.PERM_MATERIAL is null) return NoContent(); // 자재메뉴 권한
                if(dto.PERM_ENERGY is null) return NoContent(); // 에너지메뉴 권한
                if(dto.PERM_USER is null) return NoContent(); // 사용자메뉴 권한
                if(dto.PERM_VOC is null) return NoContent(); // VOC메뉴 권한
                if(dto.ALRAM_YN is null) return NoContent(); // 알람여부
                if(dto.STATUS is null) return NoContent(); // 재직여부
                if(dto.VOC_MACHINE is null) return NoContent(); // VOC 기계권한
                if(dto.VOC_ELEC is null) return NoContent(); // VOC 전기권한
                if(dto.VOC_LIFT is null) return NoContent(); // VOC 승강권한
                if(dto.VOC_FIRE is null) return NoContent(); // VOC 소방권한
                if(dto.VOC_CONSTRUCT is null) return NoContent(); // VOC 건축권한
                if(dto.VOC_NETWORK is null) return NoContent(); // VOC 통신권한
                if(dto.VOC_BEAUTY is null) return NoContent(); // VOC 미화권한
                if (dto.VOC_SECURITY is null) return NoContent(); // VOC 보안권한
                
                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseModel<UsersDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseModel<UsersDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                dto.USERID = CommService.getRemoveWhiteSpace(dto.USERID);
                dto.PASSWORD = CommService.getRemoveWhiteSpace(dto.PASSWORD);

                ResponseModel<UsersDTO> model = await UserService.AddUserService(dto, files).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailUser")]
        public async Task<IActionResult> DetailUser([FromQuery][Required]int id)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<UsersDTO> model = await UserService.GetUserDetails(id, isMobile).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody][Required] List<int> delIdx)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count == 0)
                    return NoContent();

                ResponseModel<bool?> model = await UserService.DeleteUserService(delIdx).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.ID is null) return NoContent();
                if (String.IsNullOrWhiteSpace(dto.USERID)) return NoContent(); // 사용자ID
                if (String.IsNullOrWhiteSpace(dto.PASSWORD)) return NoContent(); // 사용자 비밀번호
                if (dto.PERM_BASIC is null) return NoContent(); // 기본정보메뉴 권한
                if (dto.PERM_MACHINE is null) return NoContent(); // 기계메뉴 권한
                if (dto.PERM_ELEC is null) return NoContent(); // 전기메뉴 권한
                if (dto.PERM_LIFT is null) return NoContent(); // 승강메뉴 권한
                if (dto.PERM_FIRE is null) return NoContent(); // 소방메뉴 권한
                if (dto.PERM_CONSTRUCT is null) return NoContent(); // 건축메뉴 권한
                if (dto.PERM_NETWORK is null) return NoContent(); // 통신메뉴 권한
                if (dto.PERM_BEAUTY is null) return NoContent(); // 미화메뉴 권한
                if (dto.PERM_SECURITY is null) return NoContent(); // 보안메뉴 권한
                if (dto.PERM_MATERIAL is null) return NoContent(); // 자재메뉴 권한
                if (dto.PERM_ENERGY is null) return NoContent(); // 에너지메뉴 권한
                if (dto.PERM_USER is null) return NoContent(); // 사용자메뉴 권한
                if (dto.PERM_VOC is null) return NoContent(); // VOC메뉴 권한
                if (dto.ALRAM_YN is null) return NoContent(); // 알람여부
                if (dto.STATUS is null) return NoContent(); // 재직여부
                if (dto.VOC_MACHINE is null) return NoContent(); // VOC 기계권한
                if (dto.VOC_ELEC is null) return NoContent(); // VOC 전기권한
                if (dto.VOC_LIFT is null) return NoContent(); // VOC 승강권한
                if (dto.VOC_FIRE is null) return NoContent(); // VOC 소방권한
                if (dto.VOC_CONSTRUCT is null) return NoContent(); // VOC 건축권한
                if (dto.VOC_NETWORK is null) return NoContent(); // VOC 통신권한
                if (dto.VOC_BEAUTY is null) return NoContent(); // VOC 미화권한
                if (dto.VOC_SECURITY is null) return NoContent(); // VOC 보안권한

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseModel<UsersDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseModel<UsersDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                dto.USERID = CommService.getRemoveWhiteSpace(dto.USERID);
                dto.PASSWORD = CommService.getRemoveWhiteSpace(dto.PASSWORD);

                ResponseModel<UsersDTO>? model = await UserService.UpdateUserService(dto, files).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/UserIdCheck")]
        public async Task<IActionResult> UserIdCheck([FromQuery][Required] string userid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (String.IsNullOrWhiteSpace(userid))
                    return BadRequest();

                ResponseModel<bool?> model = await AdminAccountService.UserIdCheckService(CommService.getRemoveWhiteSpace(userid)).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
