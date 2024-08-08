using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Building
{
    /// <summary>
    /// 건물 리스트
    /// </summary>
    public class BuildinglistDTO
    {
        /// <summary>
        /// 건물 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 ID는 공백일 수 없습니다.")]
        public int ID { get; set; }

        /// <summary>
        /// 건물 코드
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 코드는 공백일 수 없습니다.")]
        public string BuildingCD { get; set; } = null!;

        /// <summary>
        /// 건물명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 건물주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public string? CompletionDT { get; set; }

        /// <summary>
        /// 등록일자
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "등록일자는 공백일 수 없습니다.")]
        public DateTime CreateDT { get; set; }

    }
}
