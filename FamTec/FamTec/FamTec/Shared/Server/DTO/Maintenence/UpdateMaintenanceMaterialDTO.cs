namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 유지보수 - 품목 입출고 내용 수정
    /// </summary>
    public class UpdateMaintenanceMaterialDTO
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int MaintanceID { get; set; }

        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 공간 ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 사용내역 ID
        /// </summary>
        public int UseMaintanceID { get; set; }

        /// <summary>
        /// 입출고 이력 ID
        /// </summary>
        public int StoreID { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 총 금액
        /// </summary>
        public float TotalPrice { get; set; }

    }
}
