using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Floor
{
    public class FloorDTO
    {
        /// <summary>
        /// 층 INDEX
        /// </summary>
        public int FloorID { get; set; }

        /// <summary>
        /// 층 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 건물 테이블 인덱스
        /// </summary>
        public int? BuildingTBID { get; set; }
    }
}
