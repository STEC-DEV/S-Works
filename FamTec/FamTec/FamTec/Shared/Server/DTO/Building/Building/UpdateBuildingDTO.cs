using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Building
{
    /// <summary>
    /// 업데이터 건물항목
    /// </summary>
    public class UpdateBuildingDTO
    {
        /// <summary>
        /// 건물 테이블 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 인덱스는 공백일 수 없습니다.")]
        public int Id { get; set; }

        /// <summary>
        /// 건물코드
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물코드는 공백일 수 없습니다.")]
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
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 건물용도
        /// </summary>
        public string? Usage { get; set; }

        /// <summary>
        /// 시공업체
        /// </summary>
        public string? ConstComp { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public DateTime? CompletionDT { get; set; }

        /// <summary>
        /// 건물구조
        /// </summary>
        public string? BuildingStruct { get; set; }

        /// <summary>
        /// 지붕구조
        /// </summary>
        public string? RoofStruct { get; set; }

        /// <summary>
        /// 첨부파일
        /// </summary>
        public string? Image { get; set; }
    }
}
