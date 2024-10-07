namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class DayEnergyDTO
    {
        public List<DayTotalEnergyDTO> TotalList { get; set; } = new List<DayTotalEnergyDTO>();

        /// <summary>
        /// 총합계
        /// </summary>
        public float? TotalPrice { get; set; }
    }

    public class DayTotalEnergyDTO
    {
        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int MeterID { get; set; }

        /// <summary>
        /// 검침기 이름
        /// </summary>
        public string? MeterName { get; set; }

        /// <summary>
        /// 계약종별 인덱스
        /// </summary>
        public int ContractID { get; set; }

        /// <summary>
        /// 계약종별 이름
        /// </summary>
        public string? ContractName { get; set; }


        /// <summary>
        /// 일자
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 총 합계
        /// </summary>
        public float? DaysUseAmount { get; set; }
    }
}
