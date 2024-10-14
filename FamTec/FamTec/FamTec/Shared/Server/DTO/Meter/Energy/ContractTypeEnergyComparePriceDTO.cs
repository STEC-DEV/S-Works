namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class ContractTypeEnergyComparePriceDTO
    {
        /// <summary>
        /// 계약종별 ID
        /// </summary>
        public int ContractTypeId { get; set; }

        /// <summary>
        /// 계약종별 이름
        /// </summary>
        public string? ContractTypeName { get; set; }

        /// <summary>
        /// 이번달 단가
        /// </summary>
        public float ThisMonthPrice { get; set; }

        /// <summary>
        /// 지난달 단가
        /// </summary>
        public float BeforeMonthPrice { get; set; }

        /// <summary>
        /// 작년 동월 단가
        /// </summary>
        public float LastYearSameMonthPrice { get; set; }
    }
}
