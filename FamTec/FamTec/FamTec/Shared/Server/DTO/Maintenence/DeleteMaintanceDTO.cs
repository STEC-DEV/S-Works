namespace FamTec.Shared.Server.DTO.Maintenence
{
    public class DeleteMaintanceDTO
    {
        /// <summary>
        /// 유지보수 이력ID
        /// </summary>
        public int? MaintanceID { get; set; }

        /// <summary>
        /// 공간 ID - 재입고시 필요
        /// </summary>
        //public int? RoomTBID { get; set; }

        /// <summary>
        /// 자재ID - 재입고시 필요
        /// </summary>
        //public int? MaterialTBID { get; set; }

        /// <summary>
        /// 유지보수용 출고 ID - 재입고시 필요
        /// </summary>
        public int? UseMaintenenceID { get; set; }

        /// <summary>
        /// 취소사유
        /// </summary>
        public string? Note { get; set; }

    }
}
