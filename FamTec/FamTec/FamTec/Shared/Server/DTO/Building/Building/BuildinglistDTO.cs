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
        public int? ID { get; set; }

        /// <summary>
        /// 건물 코드
        /// </summary>
        public string? BuildingCD { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

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
        public DateTime? CreateDT { get; set; }

    }
}
