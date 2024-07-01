using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Material
{
    public class AddMaterialDTO
    {

        /// <summary>
        /// 자재명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 자재위치
        /// </summary>
        public string? Default_Location { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? Mfr { get; set; }

        /// <summary>
        /// 안전재고 수량
        /// </summary>
        public int? SafeNum { get; set; }

        /// <summary>
        /// 건물인덱스
        /// </summary>
        public int? BuildingId { get; set; }

    }
}
