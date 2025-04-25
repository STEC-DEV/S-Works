using FamTec.Server.Helpers;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using FamTec.Shared.Server.DTO.Place;
using FamTec.Shared.Server.DTO.User;

namespace FamTec.Server.Services.User
{
    public interface IUserService
    {
        /// <summary>
        /// 일반화면 가이드 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<byte[]?> DownloadUserGuidForm();

        /// <summary>
        /// 사용자 엑셀양식 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<byte[]?> DownloadUserForm();

        /// <summary>
        /// 사용자 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool>> ImportUserService(IFormFile? file);


        /// <summary>
        /// QR로그인
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<string?>> GetQRLogin(QRLoginDTO dto);

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Task<string?> RefreshTokenService(int placeid, int useridx, bool isAdmin);

        /// <summary>
        /// 일반페이지 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<string?>> UserLoginService(LoginDTO dto);

        /// <summary>
        /// 관리자가 일반페이지 접속시 사업장 선택 후 토큰 발급되는 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<ResponseModel<string?>> LoginSelectPlaceService(int placeid);

        /// <summary>
        /// 해당사업장의 USERLIST 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<ListUser>>> GetPlaceUserList();

        /// <summary>
        /// 사용자 상세정보 보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResponseModel<UsersDTO>> GetUserDetails(int id, bool isMobile);


        /// <summary>
        /// 사용자 추가 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<UsersDTO>> AddUserService(UsersDTO dto, IFormFile? files);

        /// <summary>
        /// 사용자 삭제 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteUserService(List<int> del);

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<UsersDTO>> UpdateUserService(UsersDTO dto, IFormFile? files);

        /// <summary>
        /// 사업장 메뉴권한 리턴
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<PlacePermissionDTO?>> GetMenuPermService();

        // --------------- V2

        /// <summary>
        /// [웹] - 액세스 토큰 발급서비스 [V2]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2>?> WebUserLoginService(LoginDTO dto);

        /// <summary>
        /// [웹] QR로그인 [V2]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2>?> WebQRLoginService(QRLoginDTO dto);

        /// <summary>
        /// [웹] - 관리자가 일반페이지 접속했을때 선택한 사업장 포함한 액세스 토큰 재생성
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2?>> WebLoginSelectPlaceService(int placeid, string sessionId);

        /// <summary>
        /// [웹] - 재발급 토큰 서비스 [V2]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<TokenDTOV2>?> WebLoginRefreshTokenService(RefreshTokenDTOV2 dto);

        /// <summary>
        /// [웹] - 로그아웃 [V2]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool>> WebLogoutService(LogoutDTO dto);

    
    }
}
