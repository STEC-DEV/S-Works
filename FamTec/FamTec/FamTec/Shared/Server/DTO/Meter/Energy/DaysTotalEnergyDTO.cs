namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class DayEnergyDTO
    {
        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int MeterID { get; set; }
        
        /// <summary>
        /// 계약종별
        /// </summary>
        public string? ContractName { get; set; }

        public string? Name { get; set; }

        public float? MeterUseAmountSum { get; set; }

        /// <summary>
        /// 총합계
        /// </summary>
        public float? TotalPrice { get; set; }

        public List<DayTotalEnergyDTO> TotalList { get; set; } = new List<DayTotalEnergyDTO>();
    }

    public class DayTotalEnergyDTO
    {
        public int MeterID { get; set; }
        public string? ContractName { get; set; }

        public string? Name { get; set; }

        /// <summary>
        /// 일자
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 총 합계
        /// </summary>
        public float? TotalUseAmount { get; set; }
    }
}
