using FamTec.Server.Tokens;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Login;

namespace FamTec.Server.Services.Admin.Account
{
    public interface IAdminAccountService
    {
        /// <summary>
        /// 관리자 설정화면 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string>> AdminLoginService(LoginDTO? dto);

        /// <summary>
        /// 관리자 아이디 생성 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> AdminRegisterService(HttpContext? context, AddManagerDTO? dto);

        /// <summary>
        /// 매니저 삭제
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int>> DeleteAdminService(List<int> adminid);

       

        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<DManagerDTO>> DetailAdminService(int? adminidx);

    }
}
