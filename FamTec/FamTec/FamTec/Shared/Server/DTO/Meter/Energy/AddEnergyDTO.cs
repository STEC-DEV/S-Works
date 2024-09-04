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
        /// 소계
        /// </summary>
        public float Amount1 { get; set; }

        /// <summary>
        /// 중간부하
        /// </summary>
        public float Amount2 { get; set; }
        
        /// <summary>
        /// 최대부하
        /// </summary>
        public float Amount3 { get; set; }

        /// <summary>
        /// 합계
        /// </summary>
        public float TotalAmount { get; set;}

    
    }
}
