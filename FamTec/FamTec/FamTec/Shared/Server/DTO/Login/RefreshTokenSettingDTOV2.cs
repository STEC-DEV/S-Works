using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Login
{
    public class RefreshTokenSettingDTOV2
    {
        /// <summary>
        /// 유저 PID
        /// </summary>
        [Required]
        public string userIdx { get; set; } = null!;

        /// <summary>
        /// 세션 ID
        /// </summary>
        [Required]
        public string sessionId { get; set; } = null!;

        /// <summary>
        /// 재발급 토큰
        /// </summary>
        [Required]
        public string refreshToken { get; set; } = null!;
    }
}
