namespace FamTec.Server.Services.Redis
{
    /// <summary>
    /// Redis 관련
    /// </summary>
    public interface IRedisService
    {
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
