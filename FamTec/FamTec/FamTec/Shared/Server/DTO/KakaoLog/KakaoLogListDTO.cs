namespace FamTec.Shared.Server.DTO.KakaoLog
{
    /// <summary>
    /// 카카오 알림톡 성공여부 DTO
    /// </summary>
    public class KakaoLogListDTO
    {
        /// <summary>
        /// 카카오 로그 ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 알림톡 성공여부 Message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 보낸시간
        /// </summary>
        public string? CreateDT { get; set; }
        
        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 민원 ID
        /// </summary>
        public int? VocId { get; set; }
    }
}
