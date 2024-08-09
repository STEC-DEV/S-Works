namespace FamTec.Shared.Server.DTO.Alarm
{
    /// <summary>
    /// 미열람 알람 DTO
    /// </summary>
    public class AlarmDTO
    {
        /// <summary>
        /// 알람ID
        /// </summary>
        public int? AlarmID { get; set; }

        /// <summary>
        /// VOC 제목
        /// </summary>
        public string? VocTitle { get; set; }
    }
}
