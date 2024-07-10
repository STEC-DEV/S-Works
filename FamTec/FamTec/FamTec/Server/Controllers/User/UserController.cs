using Microsoft.AspNetCore.Mvc;
using FamTec.Shared.Server.DTO.User;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using Microsoft.AspNetCore.Authorization;

namespace FamTec.Server.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService UserService;

        public UserController(IUserService _userservice)
        {
            UserService = _userservice;
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
            ResponseList<ListUser>? model = await UserService.GetPlaceUserList(HttpContext);

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


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUser")]
        public async ValueTask<IActionResult> AddUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            ResponseUnit<UsersDTO>? model = await UserService.AddUserService(HttpContext, dto, files);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailUser")]
        public async ValueTask<IActionResult> DetailUser([FromQuery]int? id)
        {
            ResponseUnit<UsersDTO> model = await UserService.GetUserDetails(HttpContext, id);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DeleteUser")]
        public async ValueTask<IActionResult> DeleteUser([FromQuery] List<int> delIdx)
        {
            ResponseUnit<int?> model = await UserService.DeleteUserService(HttpContext, delIdx);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateUser")]
        public async ValueTask<IActionResult> UpdateUser([FromForm]UpdateUserDTO dto, [FromForm]IFormFile? files)
        {
            ResponseUnit<UpdateUserDTO>? model = await UserService.UpdateUserService(HttpContext, dto, files);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportUser")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile? file)
        {
            ResponseUnit<string>? model = await UserService.ImportUserService(HttpContext, file);
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
