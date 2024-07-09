using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 추가 DTO
    /// </summary>
    public class AddBuildingDTO
    {
        /// <summary>
        /// 건물코드
        /// </summary>
        public string? BuildingCD { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 주소
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
        /// 추가항목
        /// </summary>
        public List<AddGroupDTO>? SubItem { get; set; } = new List<AddGroupDTO>();
    }
}
