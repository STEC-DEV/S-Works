using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 매니저 추가 DTO
    /// </summary>
    public class AddManagerDTO
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 사용자 ID는 공백일 수 없습니다.")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// 비밀번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 사용자 비밀번호는 공백일 수 없습니다.")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }


        /// <summary>
        /// 부서INDEX
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 관리자의 부서는 공백일 수 없습니다.")]
        public int DepartmentId { get; set; }


        //public byte[]? Image { get; set; }

        //public string? ImageName { get; set; }
    }
}
