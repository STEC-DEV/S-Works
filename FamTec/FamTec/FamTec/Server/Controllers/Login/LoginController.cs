using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Account;
using FamTec.Server.Services.Admin.Place;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IAdminAccountService AdminAccountService;
        private IAdminPlaceService AdminPlaceService;
        private ILogService LogService;
        private IUserService UserService;

        public LoginController(IAdminAccountService _adminaccountservice,
            IAdminPlaceService _adminplaceservice,
            ILogService _logservice,
            IUserService _userservice)
        {
            this.AdminAccountService = _adminaccountservice;
            this.AdminPlaceService = _adminplaceservice;
            this.LogService = _logservice;
            this.UserService = _userservice;
        }

        /// <summary>
        /// 관리자 화면 로그인 [OK]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SettingLogin")]
        public async Task<IActionResult> SettingLogin([FromBody] LoginDTO dto)
        {
            try
            {
                /* 필수값 검사 */
                if (String.IsNullOrWhiteSpace(dto.UserID))
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                ResponseUnit<string?> model = await AdminAccountService.AdminLoginService(dto).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return Ok(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("QRLogin")]
        public async Task<IActionResult> QRLogin([FromBody] QRLoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserId))
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                ResponseUnit<string?> model = await UserService.GetQRLogin(dto).ConfigureAwait(false);

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
        /// 로그인 API - 모든사람 접근가능
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserID))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                ResponseUnit<string?> model = await UserService.UserLoginService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model); // 유저
                else if (model.code == 201)
                    return Ok(model);
                else
                    return Ok(model); // 유저
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 리스트 반환
        /// </summary>
        /// <returns></returns>
        /*
        [HttpGet]
        [Route("AdminSelectList")]
        public async Task<IActionResult> AdminSelectList([FromQuery]string token)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksList(HttpContext);
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
        */

        /// <summary>
        /// 관리자 들만 접근가능 할당된 사업장 LIST 반환
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager,Master,Manager")]
        [HttpGet]
        [Route("sign/AdminPlaceList")]
        public async Task<IActionResult> SelectPlaceList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksList(HttpContext).ConfigureAwait(false);
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
        /// 관리자들만 접근가능
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager,Master,Manager")]
        [HttpGet]
        [Route("sign/SelectPlace")]
        public async Task<IActionResult> SelectPlace([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<string?> model = await UserService.LoginSelectPlaceService(HttpContext, placeid).ConfigureAwait(false);

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
