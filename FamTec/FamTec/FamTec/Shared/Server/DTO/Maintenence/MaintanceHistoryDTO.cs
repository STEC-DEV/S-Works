namespace FamTec.Shared.Server.DTO.Maintenence
{
    public class AllMaintanceHistoryDTO
    {
        /// <summary>
        /// 년
        /// </summary>
        public int? Years { get; set; }

        /// <summary>
        /// 월
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// 유지보수 이력 DTO
        /// </summary>
        public List<MaintanceHistoryDTO> HistoryList { get; set; } = new List<MaintanceHistoryDTO>();

    }

    /// <summary>
    /// 유지보수 이력 List
    /// </summary>
    public class MaintanceHistoryDTO
    {
        /// <summary>
        /// 설비유형
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 작업일자
        /// </summary>
        public string? CreateDT { get; set; }

        /// <summary>
        /// 유비보수 설명
        /// </summary>
        public string? HistoryTitle { get; set; }

        /// <summary>
        /// 작업구분 - 자체 or 외주
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 사용자재 List
        /// </summary>
        public List<HistoryMaterialDTO> HistoryMaterialList { get; set; } = new List<HistoryMaterialDTO>();

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 금액
        /// </summary>
        public float? TotalPrice { get; set; }
        
    }

    /// <summary>
    /// 사용자재 List
    /// </summary>
    public class HistoryMaterialDTO
    {
        /// <summary>
        /// 사용자재 ID
        /// </summary>
        public int? MaterialID { get; set; }
        
        /// <summary>
        /// 사용자재 명
        /// </summary>
        public string? MaterialName { get; set; }
    }

}
