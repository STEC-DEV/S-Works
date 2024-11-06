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
        /// QR로그인
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<string?>> GetQRLogin(QRLoginDTO dto);

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
        public Task<ResponseUnit<string?>> UserLoginService(LoginDTO dto);

        /// <summary>
        /// 관리자가 일반페이지 접속시 사업장 선택 후 토큰 발급되는 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<string?>> LoginSelectPlaceService(HttpContext context, int placeid);

        /// <summary>
        /// 해당사업장의 USERLIST 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<ResponseList<ListUser>> GetPlaceUserList(HttpContext context);

        /// <summary>
        /// 사용자 상세정보 보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UsersDTO>> GetUserDetails(HttpContext context, int id, bool isMobile);


        /// <summary>
        /// 사용자 추가 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UsersDTO>> AddUserService(HttpContext context, UsersDTO dto, IFormFile? files);

        /// <summary>
        /// 사용자 삭제 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteUserService(HttpContext context, List<int> del);

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UsersDTO>> UpdateUserService(HttpContext context, UsersDTO dto, IFormFile? files);

        /// <summary>
        /// 사용자 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ResponseUnit<string?>> ImportUserService(HttpContext context, IFormFile? file);

        /// <summary>
        /// 사업장 메뉴권한 리턴
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseUnit<PlacePermissionDTO?>> GetMenuPermService(HttpContext context);
    }
}
