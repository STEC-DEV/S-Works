namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class EnergyMonthChargePriceDTO
    {
        /// <summary>
        /// 대상 날짜 - 년 / 월
        /// </summary>
        public DateTime TargetDate { get; set; }
        
        /// <summary>
        /// 청구금액
        /// </summary>
        public float ChargePrice { get; set; }
    }
}
