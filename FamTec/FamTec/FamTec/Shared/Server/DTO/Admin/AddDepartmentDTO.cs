using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 부서 추가DTO
    /// </summary>
    public class AddDepartmentDTO
    {
        /// <summary>
        /// 부서 명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가할 부서명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 관리부서 여부
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "관리부서 여부는 공백일 수 없습니다.")]
        public bool ManagerYN { get; set; }
    }
}
