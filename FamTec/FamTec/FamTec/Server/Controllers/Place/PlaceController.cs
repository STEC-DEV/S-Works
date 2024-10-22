using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Place;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Place
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IUserService UserService;
        private readonly IAdminPlaceService AdminPlaceService;

        private readonly ILogService LogService;
        private readonly ConsoleLogService<PlaceController> CreateBuilderLogger;
        
        public PlaceController(IAdminPlaceService _adminplaceservice,
            IUserService _userservice,
            ILogService _logservice,
            ConsoleLogService<PlaceController> _createbuilderlogger)
        {
            this.AdminPlaceService = _adminplaceservice;
            this.UserService = _userservice;

            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [HttpGet]
        [Route("GetPlaceName")]
        public async Task<IActionResult> GetPlaceName([FromQuery]int placeid)
        {
            try
            {
                if (placeid is 0)
                    return NoContent();

                ResponseUnit<string?> model = await AdminPlaceService.GetPlaceName(placeid).ConfigureAwait(false);
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


        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlacePerm")]
        public async Task<IActionResult> GetPlacePerm()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<PlacePermissionDTO?> model = await UserService.GetMenuPermService(HttpContext);
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
