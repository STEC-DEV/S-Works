namespace FamTec.Shared.Server.DTO.KakaoLog
{
    public class AuthCodeCheckDTO
    {
        /// <summary>
        /// 전화번호
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 인증코드
        /// </summary>
        public string? AuthCode { get; set; }
    }
}
