using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    /// <summary>
    /// 사업장 등록 화면 DTO
    /// </summary>
    public class AddPlaceDTO
    {
        /// <summary>
        /// 사업장코드
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사업장 코드는 공백일 수 없습니다.")]
        public string PlaceCd { get; set; } = null!;

        /// <summary>
        /// 사업장 명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사업장 명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 전화번호
        /// </summary>
        /// [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "숫자만 입력 가능합니다.")]
        public string? Tel { get; set; }

        /// <summary>
        /// 사업장 주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        public string? ContractNum { get; set; }

        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDT { get; set; }

        /// <summary>
        /// 기계메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "기계메뉴 권한은 공백일 수 없습니다.")]
        public bool PermMachine { get; set; } = false;

        /// <summary>
        /// 승강메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "승강메뉴 권한은 공백일 수 없습니다.")]
        public bool PermLift { get; set; } = false;

        /// <summary>
        /// 소방메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "소방메뉴 권한은 공백일 수 없습니다.")]
        public bool PermFire { get; set; } = false;

        /// <summary>
        /// 건축메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건축메뉴 권한은 공백일 수 없습니다.")]
        public bool PermConstruct { get; set; } = false;

        /// <summary>
        /// 통신메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "통신메뉴 권한은 공백일 수 없습니다.")]
        public bool PermNetwork { get; set; } = false;

        /// <summary>
        /// 미화 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "미화메뉴 권한은 공백일 수 없습니다.")]
        public bool PermBeauty { get; set; } = false;

        /// <summary>
        /// 보안메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "보안메뉴 권한은 공백일 수 없습니다.")]
        public bool PermSecurity { get; set; } = false;

        /// <summary>
        /// 자재메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "자재메뉴 권한은 공백일 수 없습니다.")]
        public bool PermMaterial { get; set; } = false;

        /// <summary>
        /// 에너지메뉴 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "에너지메뉴 권한은 공백일 수 없습니다.")]
        public bool PermEnergy { get; set; } = false;

        /// <summary>
        /// VOC 권한
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "민원메뉴 권한은 공백일 수 없습니다.")]
        public bool PermVoc { get; set; } = false;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "계약상태는 공백일 수 없습니다.")]
        public bool Status { get; set; } = true;

        /// <summary>
        /// 관리부서 인덱스
        /// </summary>
        public int DepartmentID { get; set; }
    }
}