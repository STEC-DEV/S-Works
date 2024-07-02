using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 추가항목 DTO
    /// </summary>
    public class BuildingSubItemDTO
    {
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
