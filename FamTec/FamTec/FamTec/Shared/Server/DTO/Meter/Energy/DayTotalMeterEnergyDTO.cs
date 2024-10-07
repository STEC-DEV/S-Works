namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class DayTotalMeterEnergyDTO
    {
        /// <summary>
        /// 계약종별 ID
        /// </summary>
        public int ContractTypeId { get; set; }

        /// <summary>
        /// 계약종별 명칭
        /// </summary>
        public string? ContractTypeName { get; set; }

        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// 검침기 명칭
        /// </summary>
        public string? MeterName { get; set; }

        /// <summary>
        /// 월합계 사용량
        /// </summary>
        public float? TotalUse { get; set; }

        public List<DayTotalUse> DayTotalUseList { get; set; } = new List<DayTotalUse>();
    }


}
