using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Login
{
    public class TokenDTOV2
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [Required]
        public string accessToken { get; set; } = null!;

        /// <summary>
        /// 동시로그인 허용하기 위해
        /// </summary>
        public string? sessionId { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [Required]
        public string refreshToken { get; set; } = null!;
    }
}
