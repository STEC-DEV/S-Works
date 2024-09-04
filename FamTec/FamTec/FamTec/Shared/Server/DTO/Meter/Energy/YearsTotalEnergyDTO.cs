namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class YearsTotalEnergyDTO
    {
        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int MeterID { get; set; }

        /// <summary>
        /// 계약종별
        /// </summary>
        public string? ContractType { get; set; }

        /// <summary>
        /// 검침기 명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 1월 통계
        /// </summary>
        public float JAN { get; set; }

        /// <summary>
        /// 2월 통계
        /// </summary>
        public float FEB { get; set; }

        /// <summary>
        /// 3월 통계
        /// </summary>
        public float MAR { get; set; }

        /// <summary>
        /// 4월 통계
        /// </summary>
        public float APR { get; set; }

        /// <summary>
        /// 5월 통계
        /// </summary>
        public float MAY { get; set; }

        /// <summary>
        /// 6월 통계
        /// </summary>
        public float JUN { get; set; }

        /// <summary>
        /// 7월 통계
        /// </summary>
        public float JUL { get; set; }

        /// <summary>
        /// 8월 통계
        /// </summary>
        public float AUG { get; set; }

        /// <summary>
        /// 9월 통계
        /// </summary>
        public float SEP { get; set; }

        /// <summary>
        /// 10월 통계
        /// </summary>
        public float OCT { get; set; }

        /// <summary>
        /// 11월 통계
        /// </summary>
        public float NOV { get; set; }

        /// <summary>
        /// 12월 통계
        /// </summary>
        public float DEC { get; set; }
    }
}
