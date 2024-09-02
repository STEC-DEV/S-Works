namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    /// <summary>
    /// 일일 검침 입력 DTO
    /// </summary>
    public class AddEnergyDTO
    {
        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int MeterID { get; set; }
        
        /// <summary>
        /// 검침일
        /// </summary>
        public DateTime MeterDate { get; set; }
        
        /// <summary>
        /// 검침값
        /// </summary>
        public float UseAmount { get; set; }
    }
}
