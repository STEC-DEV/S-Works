using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class MaintenanceListDTO
    {
        /// <summary>
        /// 유지보수 이력 인덱스
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 일자
        /// </summary>
        public DateTime? CreateDT { get; set; }

        /// <summary>
        /// 유지보수 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 작업구분
        /// </summary>
        public int? Type { get; set; }


        /// <summary>
        /// 총 비용
        /// </summary>
        public float? TotalPrice { get; set; }
    }
}
