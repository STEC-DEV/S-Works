namespace FamTec.Shared.Server.DTO.Maintenence
{
    //MaintanceID / 사진 / 작업명 / 작업자
    public class UpdateMaintenanceDTO
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 작업명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }
    }
}
 