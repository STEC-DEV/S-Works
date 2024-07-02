using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 추가항목 상세정보 DTO
    /// </summary>
    public class DetailBuildingSubItemDTO
    {
        /// <summary>
        /// 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 항목명
        /// </summary>
        public string? ItemName { get; set; }

        /// <summary>
        /// 단위명
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 값
        /// </summary>
        public string? Value { get; set; } // 값
    }
}
