using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Login
{
    public class LoginDTO
    {
        /// <summary>
        /// 로그인 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "로그인 ID는 공백일 수 없습니다.")]
        public string UserID { get; set; } = null!;

        /// <summary>
        /// 로그인 PASSWORD
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "로그인 비밀번호는 공백일 수 없습니다.")]
        public string UserPassword { get; set; } = null!;
    }
}
