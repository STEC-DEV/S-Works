using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class DetailMaintenanceDTO
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int MaintanceID { get; set; }

        /// <summary>
        /// 작업일자
        /// </summary>
        public string? WorkDT { get; set; }

        /// <summary>
        /// 작업명칭
        /// </summary>
        public string? WorkName { get; set; }

        /// <summary>
        /// 작업구분
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 소요비용
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 이미지
        /// </summary>
        public byte[]? Image { get; set; }

        public List<UseStoreDTO> UseStoreList { get; set; } = new List<UseStoreDTO>();

    }
}
