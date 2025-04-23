using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Login
{
    public class RefreshTokenDTOV2
    {
        /// <summary>
        /// 유저 PID
        /// </summary>
        [Required]
        public string UserIdx { get; set; } = null!;

        /// <summary>
        /// 사업장 ID
        /// </summary>
        [Required]
        public string PlaceIdx { get; set; } = null!;

        [Required]
        public string sessionId { get; set; } = null!;

        /// <summary>
        /// 재발급 토큰
        /// </summary>
        [Required]
        public string refreshToken { get; set; } = null!;
    }
}
