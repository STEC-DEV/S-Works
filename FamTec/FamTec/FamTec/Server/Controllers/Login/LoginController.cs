using FamTec.Server.Middleware;
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
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAdminAccountService AdminAccountService;
        private readonly IAdminPlaceService AdminPlaceService;
        private readonly IUserService UserService;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<LoginController> CreateBuilderLogger;

        public LoginController(IAdminAccountService _adminaccountservice,
            IAdminPlaceService _adminplaceservice,
            IUserService _userservice,
            ILogService _logservice,
            //IDapperTempRepository _dappertemp,
            ConsoleLogService<LoginController> _createbuilderlogger)
        {
            this.AdminAccountService = _adminaccountservice;
            this.AdminPlaceService = _adminplaceservice;
            this.UserService = _userservice;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 402)
                    return Ok(model);
                else if (model.code == 403)
                    return Ok(model);
                else
                    return Ok(model);
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

        
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshTokten([FromBody]RefreshTokenDTO token)
        {
            try
            {
                if(token.placeid is 0)
                    return NoContent();

                if (token.useridx is 0)
                    return NoContent();

                string? refreshtoken = await UserService.RefreshTokenService(token.placeid!.Value, token.useridx!.Value, token.isAdmin).ConfigureAwait(false);
                if (!String.IsNullOrWhiteSpace(refreshtoken))
                    return Ok(refreshtoken);
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
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

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model); // 유저
                else if (model.code == 201)
                    return Ok(model);
                else if (model.code == 400) // 아이디-비밀번호가 틀렸을경우
                    return Ok(model);
                else
                    return Ok(model); // 유저
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
                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksList().ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
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
                ResponseUnit<string?> model = await UserService.LoginSelectPlaceService(placeid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
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


        ///
        /// -------------------- V2
        ///

        /// <summary>
        /// 웹 로그인 - V2 [Redis 캐시]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v2/Web/Login")]
        public async Task<IActionResult> WebLoginV2([FromBody] LoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserID))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                ResponseUnit<TokenDTOV2>? model = await UserService.WebUserLoginService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model); // 유저
                else if (model.code == 201) // 관리자 로그인했을 경우 사업장 선택화면으로 이동해야함.
                    return Ok(model);
                else if (model.code == 403)
                    return Ok(model);
                else if (model.code == 404) // 아이디-비밀번호가 틀렸을경우
                    return Ok(model);
                else
                    return Ok(model); // 유저
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

        /// <summary>
        /// 관리자들만 접근가능
        /// - 웹용 사업장 선택 토큰 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager,Master,Manager")]
        [HttpGet]
        [Route("sign/v2/Web/UserSelectPlace")]
        public async Task<IActionResult> WebSelectPlaceV2([FromQuery] int placeid, [FromQuery]string sessionId)
        {
            try
            {
                if (placeid is 0)
                    return BadRequest();
                
                if (String.IsNullOrWhiteSpace(sessionId))
                    return BadRequest();

                var model = await UserService.WebLoginSelectPlaceService(placeid, sessionId).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 403)
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

        /// <summary>
        /// 토큰 재발급
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("v2/Web/UserRefreshToken")]
        public async Task<IActionResult> WebRefreshTokenV2([FromBody]RefreshTokenDTOV2 dto)
        {
            try
            {
                var model = await UserService.WebLoginRefreshTokenService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 403)
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
        /// 로그아웃
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("sign/v2/Web/Logout")]
        public async Task<IActionResult> WebLogOutV2([FromBody]LogoutDTO dto)
        {
            try
            {
                var model = await UserService.WebLogoutService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 403)
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
        /// QR 로그인 - V2
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v2/Web/QRLogin")]
        public async Task<IActionResult> WebQRLoginV2([FromBody] QRLoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserId))
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                var model = await UserService.WebQRLoginService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else if (model.code == 403)
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

        [HttpPost]
        [Route("v2/web/SettingLogin")]
        public async Task<IActionResult> SettingLoginV2([FromBody] LoginDTO dto)
        {
            try
            {
                /* 필수값 검사 */
                if (String.IsNullOrWhiteSpace(dto.UserID))
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return NoContent();

                var model = await AdminAccountService.WebAdminLoginService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 402)
                    return Ok(model);
                else if (model.code == 403)
                    return Ok(model);
                else
                    return Ok(model);
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

        /// <summary>
        /// 토큰 재발급
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("v2/Web/SettingRefreshToken")]
        public async Task<IActionResult> WebRefreshTokenV2([FromBody] RefreshTokenSettingDTOV2 dto)
        {
            try
            {
                var model = await AdminAccountService.WebAdminLoginRefreshTokenService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 403)
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
