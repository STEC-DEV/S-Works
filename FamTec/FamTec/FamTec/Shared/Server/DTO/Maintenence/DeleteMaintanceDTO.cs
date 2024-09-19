namespace FamTec.Shared.Server.DTO.Maintenence
{
    public class DeleteMaintanceDTO
    {
        /// <summary>
        /// 유지보수 이력ID
        /// </summary>
        public int? MaintanceID { get; set; }
        /// <summary>
        /// 취소사유
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 유지보수용 사용자재 ID
        /// </summary>
        public List<int> UseMaintenenceIDs { get; set; } = new List<int>();
    }



}
