using FamTec.Shared.Server.DTO.Login;

namespace FamTec.Server.Services.Redis
{
    /// <summary>
    /// Redis 관련
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 액세스 토큰 저장 [유저페이지 - 유저]
        /// </summary>
        /// <returns></returns>
        Task<(string, string, string)?> SetWebUserpageAccessAsync(int pId, string accesstoken, string? sessionId = null);

        /// <summary>
        /// 액세스 토큰 저장 [세팅페이지]
        /// </summary>
        /// <returns></returns>

        Task<(string, string, string)?> SetWebSettingpageAccessAsync(int pId, string accesstoken);

        /// <summary>
        /// 리프레쉬 토큰 재발급 [유저페이지]
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="refreshtoken"></param>
        /// <returns></returns>
        Task<string?> WebRotateUserpageRefreshTokenAsync(int pId, string refreshtoken, string sessionId);

        /// <summary>
        /// 리프레쉬 토큰 재발급 [세팅페이지]
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="refreshtoken"></param>
        /// <returns></returns>
        Task<string?> WebRotateSettingpageRefreshTokenAsync(int pId, string refreshtoken, string sessionId);

        /// <summary>
        /// 레디스 키 삭제
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        Task<bool> DeleteRefreshTokenAsync(int pidm, string sessionId);

        /// <summary>
        /// 휴대전화 번호에 해당하는 인증키 Redis 서버에 저장
        /// </summary>
        /// <returns></returns>
        Task<string> SetCodeAsync(string phoneNumber);

        /// <summary>
        /// 휴대전화 번호와 비교해 검증
        /// 정상 시 true, 실패 시 false
        /// 검증 후 해당 키 삭제
        /// </summary>
        /// <returns></returns>
        Task<bool> GetValidateCodeAsync(string phoneNumber, string code);

    }
}
