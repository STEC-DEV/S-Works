using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 관리자 수정 DTO
    /// </summary>
    public class UpdateManagerDTO
    {
        /// <summary>
        /// 관리자 테이블ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "관리자 인덱스는 공백일 수 없습니다.")]
        public int AdminIndex { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "변경하실 이름은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 부서인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "변경하실 부서 인덱스는 공백일 수 없습니다.")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 로그인 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "변경하실 아이디는 공백일 수 없습니다.")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// 비밀번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "변경하실 비밀번호는 공백일 수 없습니다.")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 사업장
        /// </summary>
        public List<AdminPlaceDTO> PlaceList { get; set; } = new List<AdminPlaceDTO>();

    }
}
