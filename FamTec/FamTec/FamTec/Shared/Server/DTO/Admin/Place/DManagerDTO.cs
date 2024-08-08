using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class DManagerDTO
    {
        /// <summary>
        /// 관리자 이름
        /// </summary>
        [Display(Name = "이름")]
        public string? Name { get; set; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사용자 아이디는 공백일 수 없습니다.")]
        [Display(Name = "아이디")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// 관리자 비밀번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "비밀번호는 공백일 수 없습니다.")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 관리자 이메일
        /// </summary>
        [Display(Name = "이메일")]
        public string? Email { get; set; }

        /// <summary>
        /// 관리자 전화번호
        /// </summary>
        [Display(Name = "전화번호")]
        public string? Phone { get; set; }

        /// <summary>
        /// 관리자 계정타입
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "계정유형은 공백일 수 없습니다.")]
        [Display(Name = "계정 유형")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// 관리자 부서명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서명은 공백일 수 없습니다.")]
        [Display(Name = "부서")]
        public string Department { get; set; } = null!;

        /// <summary>
        /// 관리자 이미지
        /// </summary>
        public byte[]? Image { get; set; }
    }
}
