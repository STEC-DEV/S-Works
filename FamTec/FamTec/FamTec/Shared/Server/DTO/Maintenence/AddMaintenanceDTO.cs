using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Http;

namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 유지보수 등록
    /// </summary>
    public class AddMaintenanceDTO
    {
        /// <summary>
        /// 작업이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 자체작업 0 : 외주작업 1
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 소요비용
        /// </summary>
        public float? TotalPrice { get; set; }
        
        /// <summary>
        /// 설비ID
        /// </summary>
        public int? FacilityID { get; set; }

        /// <summary>
        /// 작업일자
        /// </summary>
        public DateTime WorkDT { get; set; }

        /// <summary>
        /// 출고정보
        /// </summary>
        public List<InOutInventoryDTO> Inventory { get; set; } = new List<InOutInventoryDTO>();
    }
}
