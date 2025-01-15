namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class MaintanceYearPriceDTO
    {
        /// <summary>
        /// 월
        /// </summary>
        public string? Month { get; set; }

        /// <summary>
        /// 기계
        /// </summary>
        public float MachineType { get; set; }
        
        /// <summary>
        /// 전기
        /// </summary>
        public float ElecType { get; set; }

        /// <summary>
        /// 승강
        /// </summary>
        public float liftType { get; set; }

        /// <summary>
        /// 소방
        /// </summary>
        public float FireType { get; set; }

        /// <summary>
        /// 미화
        /// </summary>
        public float BeautyType { get; set; }

        /// <summary>
        /// 건축
        /// </summary>
        public float ConstructType { get; set; }

        /// <summary>
        /// 통신
        /// </summary>
        public float NetWorkType { get; set; }

        /// <summary>
        /// 보안
        /// </summary>
        public float SecurityType { get; set; }
    }
}
