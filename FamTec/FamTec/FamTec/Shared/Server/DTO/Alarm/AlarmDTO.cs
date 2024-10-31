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
        /// 0: 접수됨 / 1: 상태가 변경됨
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 민원의 타입
        /// </summary>
        public int? VocType { get; set; }

        /// <summary>
        /// VOC 제목
        /// </summary>
        public string? VocTitle { get; set; }

        /// <summary>
        /// USERID
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        /// VOC 테이블 ID
        /// </summary>
        public int? VocID { get; set; }

        /// <summary>
        /// 접수코드
        /// </summary>
        public string? CODE { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 알람 발생일
        /// </summary>
        public string? CreateDT { get; set; }
    }
}
