namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class VocDaysCountDTO
    {
        /// <summary>
        /// 미분류 카운트
        /// </summary>
        public int DefaultType { get; set; }

        /// <summary>
        /// 기계 카운트
        /// </summary>
        public int MachineType { get; set; }

        /// <summary>
        /// 전기 카운트
        /// </summary>
        public int ElecType { get; set; }

        /// <summary>
        /// 승강 카운트
        /// </summary>
        public int liftType { get; set; }

        /// <summary>
        /// 건축 카운트
        /// </summary>
        public int ConstructType { get; set; }

        /// <summary>
        /// 소방 카운트
        /// </summary>
        public int FireType { get; set; }

        /// <summary>
        /// 통신 카운트
        /// </summary>
        public int NetWorkType { get; set; }

        /// <summary>
        /// 미화 카운트
        /// </summary>
        public int BeautyType { get; set; }

        /// <summary>
        /// 보안 카운트
        /// </summary>
        public int SecurityType { get; set; }

        /// <summary>
        /// 합계
        /// </summary>
        public int TotalCount { get; set; }
    }
}
