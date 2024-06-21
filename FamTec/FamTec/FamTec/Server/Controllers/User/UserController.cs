using Microsoft.AspNetCore.Mvc;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO.User;
using FamTec.Server.Services.User;
using FamTec.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FamTec.Server.Tokens;
using Newtonsoft.Json.Linq;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using Microsoft.AspNetCore.Authorization;
using FamTec.Server.Repository.User;

namespace FamTec.Server.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService UserService;
        private ITokenComm TokenComm;


        public UserController(IUserService _userservice, ITokenComm _tokencomm)
        {
            UserService = _userservice;
            TokenComm = _tokencomm;
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
        public async ValueTask<IActionResult> AddUser([FromBody]UsersDTO dto)
        {
            ResponseUnit<UsersDTO>? model = await UserService.AddUserService(HttpContext, dto);

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
