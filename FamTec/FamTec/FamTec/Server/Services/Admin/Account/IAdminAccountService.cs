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
        public Task<ResponseUnit<string?>> AdminLoginService(LoginDTO dto);

        /// <summary>
        /// 관리자 계정 생성
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public Task<ResponseUnit<int?>> AdminRegisterService(HttpContext context, AddManagerDTO dto, IFormFile? files);

        /// <summary>
        /// 관리자 계정 삭제
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteAdminService(HttpContext context, List<int> adminidx);
       

        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DManagerDTO>> DetailAdminService(int adminidx, bool isMobile);

        /// <summary>
        /// 매니저 정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateAdminService(HttpContext context, UpdateManagerDTO dto);

        /// <summary>
        /// 매니저 이미지 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateAdminImageService(HttpContext context, int id, IFormFile? files);

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UserIdCheckService(string userid);
    }
}
