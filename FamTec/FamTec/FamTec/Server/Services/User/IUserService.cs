using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using FamTec.Shared.Server.DTO.User;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Services.User
{
    public interface IUserService
    {
        /// <summary>
        /// 일반페이지 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string>?> UserLoginService(LoginDTO? dto);

        /// <summary>
        /// 관리자가 일반페이지 접속시 사업장 선택 후 토큰 발급되는 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string>?> LoginSelectPlaceService(HttpContext context, int? placeid);

        /// <summary>
        /// 해당사업장의 USERLIST 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<ListUser>> GetPlaceUserList(HttpContext? context);


        /// <summary>
        /// 사용자 추가 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<UsersDTO>> AddUserService(HttpContext? context, UsersDTO? dto);

    }
}
