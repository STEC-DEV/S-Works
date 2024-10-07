namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class DaysTotalContractTypeEnergyDTO
    {
        /// <summary>
        /// 계약종별ID
        /// </summary>
        public int ContractTypeId { get; set; }

        /// <summary>
        /// 계약종별 이름
        /// </summary>
        public string? ContractTypeName { get; set; }

        /// <summary>
        /// 월합계 사용량
        /// </summary>
        public float? TotalUse { get; set; }

        public List<DayTotalUse> DayTotalUseList { get; set; } = new List<DayTotalUse>();
    }

    public class DayTotalUse 
    { 
        /// <summary>
        /// 일자
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 일 사용량
        /// </summary>
        public float? DaysUseAmount { get; set; }
    }


}
