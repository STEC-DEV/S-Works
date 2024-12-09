using FamTec.Server.Middleware;
using FamTec.Server.Repository.DapperTemp;
using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Account;
using FamTec.Server.Services.Admin.Place;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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

        //private readonly IDapperTempRepository DapperTemp;

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

            //this.DapperTemp = _dappertemp;
        }

        // Dapper 사용 예제
        /*
        [HttpGet]
        [Route("temp")]
        public async Task<IActionResult> Temp()
        {
            await DapperTemp.SelectUser();

            return Ok("Asdfasdf");
        }
        */

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
                //var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                //if (!string.IsNullOrEmpty(forwardedFor))
                //{
                //    var ipAddress = forwardedFor.Split(',').FirstOrDefault();
                //    Console.WriteLine(ipAddress);
                //}
                //else
                //{
                //    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                //    Console.WriteLine(ipAddress);
                //}


                /*
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // X-Forwarded-For 헤더가 있을 경우 이를 우선적으로 사용
                var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    ipAddress = forwardedFor.Split(',').FirstOrDefault();
                }
                Console.WriteLine(ipAddress);
                */

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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksList(HttpContext).ConfigureAwait(false);
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<string?> model = await UserService.LoginSelectPlaceService(HttpContext, placeid).ConfigureAwait(false);

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
        

    }
}
