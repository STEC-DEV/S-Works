using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 리스트 - QR
    /// </summary>
    public class BuildinglistDTO
    {
        // 인덱스
        public int? ID { get; set; }

        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? PlaceID { get; set; }

        // 건물이름
        public string? Name { get; set; }

        // 주소
        public string? Address { get; set; }

        // 건물층수
        public int? FloorNum { get; set; }

        // 준공년월
        public DateTime? CompletionDT { get; set; }

        // 등록일자
        public DateTime? CreateDT { get; set; }

        
    }
}
