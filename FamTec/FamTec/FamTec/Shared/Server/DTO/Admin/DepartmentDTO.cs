using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin
{
    public class DepartmentDTO
    {
        /// <summary>
        /// 부서 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서 인덱스는 공백일 수 없습니다.")]
        public int Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        public bool IsSelect { get; set; } = false;

        /// <summary>
        /// 부서명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 관리부서 여부
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "관리부서 여부는 공백일 수 없습니다.")]
        public bool ManageYN { get; set; }

        public string? Description { get; set; } = null;

    }
}
