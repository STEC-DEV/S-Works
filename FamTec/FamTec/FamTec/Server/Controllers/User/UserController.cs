using Microsoft.AspNetCore.Mvc;
using FamTec.Shared.Server.DTO.User;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using Microsoft.AspNetCore.Authorization;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService UserService;
        private ILogService LogService;

        public UserController(IUserService _userservice, ILogService _logservice)
        {
            this.UserService = _userservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 로그인한 사업장의 모든 사용자 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceUsers")]
        public async ValueTask<IActionResult> GetUserList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ListUser>? model = await UserService.GetPlaceUserList(HttpContext);

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
        /// 사용자 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUser")]
        public async ValueTask<IActionResult> AddUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                // 파일크기 1MB 지정
                if (files is not null)
                {
                    if (files.Length > 1048576)
                    {
                        return Ok(new ResponseUnit<UsersDTO>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<UsersDTO>? model = await UserService.AddUserService(HttpContext, dto, files);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailUser")]
        public async ValueTask<IActionResult> DetailUser([FromQuery]int? id)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UsersDTO> model = await UserService.GetUserDetails(HttpContext, id);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DeleteUser")]
        public async ValueTask<IActionResult> DeleteUser([FromQuery] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await UserService.DeleteUserService(HttpContext, delIdx);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateUser")]
        public async ValueTask<IActionResult> UpdateUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UsersDTO>? model = await UserService.UpdateUserService(HttpContext, dto, files);

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
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportUser")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile? file)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<string>? model = await UserService.ImportUserService(HttpContext, file);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

    }
}
