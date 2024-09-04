namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    /// <summary>
    /// 사용량 비교 DTO
    /// </summary>
    public class GetUseCompareDTO
    {
        /// <summary>
        /// 당월 사용량
        /// </summary>
        public int ThisUseMonth { get; set; }
        
        /// <summary>
        /// 전월 사용량
        /// </summary>
        public int BeforeUseMonth { get; set; }

        /// <summary>
        /// 증감 값
        /// </summary>
        public int indecrease { get; set; }

        /// <summary>
        /// 전년 동월
        /// </summary>
        public int BeforeThisUseMonth { get; set; }

        /// <summary>
        /// 당월 - 작년 당월 증감값
        /// </summary>
        public int Beforeindecrease { get; set; }
    }
}
