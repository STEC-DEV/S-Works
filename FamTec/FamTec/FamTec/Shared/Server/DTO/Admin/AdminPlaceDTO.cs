using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin
{
    public class AdminPlaceDTO
    {
        /// <summary>
        /// PlaceID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사업장 인덱스는 공백일 수 없습니다.")]
        public int Id { get; set; }

        /// <summary>
        /// PlaceCode
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사업장 코드는 공백일 수 없습니다.")]
        public string PlaceCd { get; set; } = null!;

        /// <summary>
        /// 사업장명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사업장 명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; } = null;

        /// <summary>
        /// 계약번호
        /// </summary>
        public string? ContractNum { get; set; } = null;

        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDt { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "계약상태는 공백일 수 없습니다")]
        public bool Status { get; set; }

    }
}
