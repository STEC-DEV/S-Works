using FamTec.Shared.Client.DTO.Normal.Material.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class AddMaintenanceDTO
    {
        /// <summary>
        /// 작업명
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 작업구분
        /// 자체작업 0 외주작업 1
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 작업자
        /// </summary>
        public string Worker { get; set; }
        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }
        /// <summary>
        /// 작업 설비 수량
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 소요비용
        /// </summary>
        public float TotalPrice { get; set; }
        /// <summary>
        /// 설비 id
        /// </summary>
        public int FacilityId { get; set; }
        /// <summary>
        /// 작업일자
        /// </summary>
        public DateTime? WorkDT { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
        /// <summary>
        /// 사용자재
        /// </summary>
        public List<InOutInventoryDTO> Inventory { get; set; } = new List<InOutInventoryDTO>();

    }
}
