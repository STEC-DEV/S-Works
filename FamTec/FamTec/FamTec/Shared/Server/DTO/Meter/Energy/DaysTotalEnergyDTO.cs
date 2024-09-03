namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class DaysTotalEnergyDTO
    {
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
