using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Material
{
    /// <summary>
    /// 품목 리스트 DTO
    /// </summary>
    public class MaterialListDTO
    {
        /// <summary>
        /// 자재(품목) INDEX
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 품목이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacturingComp { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 안전재고
        /// </summary>
        public int? SafeNum { get; set; }



    }
}
