using FamTec.Server.Helpers;
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
        public Task<ResponseModel<string?>> AdminLoginService(LoginDTO dto);

        /// <summary>
        /// 관리자 계정 생성
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public Task<ResponseModel<int?>> AdminRegisterService(AddManagerDTO dto, IFormFile? files);

        /// <summary>
        /// 관리자 계정 삭제
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteAdminService(List<int> adminidx);
       
        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public Task<ResponseModel<DManagerDTO>> DetailAdminService(int adminidx, bool isMobile);

        /// <summary>
        /// 매니저 정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateAdminService(UpdateManagerDTO dto);

        /// <summary>
        /// 매니저 이미지 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateAdminImageService(int id, IFormFile? files);

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UserIdCheckService(string userid);


        //------------------- V2

        /// <summary>
        /// [웹] - 관리자 화면 액세스 토큰 발급 서비스 (V2)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2>?> WebAdminLoginService(LoginDTO dto);

        /// <summary>
        /// [웹] - 관리자 화면 재발급 토큰 서비스 (V2)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2>?> WebAdminLoginRefreshTokenService(RefreshTokenSettingDTOV2 dto);

    }
}
