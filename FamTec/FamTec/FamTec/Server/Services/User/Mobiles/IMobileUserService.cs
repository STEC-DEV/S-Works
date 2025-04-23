using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;

namespace FamTec.Server.Services.User.Mobiles
{
    /// <summary>
    /// 모바일용 UserService 인터페이스
    /// </summary>
    public interface IMobileUserService
    {
        /// <summary>
        /// [모바일] 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<string?>> MobileUserLoginService(LoginDTO dto);

        public Task<ResponseUnit<string?>> MobileLoginSelectPlaceService(HttpContext context, int placeid);

    }
}
