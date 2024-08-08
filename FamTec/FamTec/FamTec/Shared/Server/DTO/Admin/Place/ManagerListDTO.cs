using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class ManagerListDTO
    {
        /// <summary>
        /// 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "ID는 공백일 수 없습니다.")]
        public int Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        [Display(Name = "선택")]
        public bool? IsSelect { get; set; } = false;

        /// <summary>
        /// 아이디
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "아이디는 공백일 수 없습니다.")]
        [Display(Name = "아이디")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// 이름
        /// </summary>
        [Display(Name = "이름")]
        public string? Name { get; set; }

        /// <summary>
        /// 부서
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서명은 공백일 수 없습니다.")]
        [Display(Name = "부서")]
        public string Department { get; set; } = null!;
    }
}
