namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class MaintenanceDaysDTO
    {
        /// <summary>
        /// 설비유형
        /// </summary>
        public string? FacilityType { get; set; }

        /// <summary>
        /// 작업구분
        /// </summary>
        public string? WorkType { get; set; }

        /// <summary>
        /// 작업내용
        /// </summary>
        public string? WorkContent { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 총 사용자재 개수
        /// </summary>
        public int TotalUseMaterialNum { get; set; }

        public List<MaintenanceUseMaterialDTO> UseList { get; set; } = new List<MaintenanceUseMaterialDTO>();
    }

    public class MaintenanceUseMaterialDTO
    {
        /// <summary>
        /// 자재 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 자재 명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 사용수량
        /// </summary>
        public int UseMaterialNum { get; set; }
    }
}
