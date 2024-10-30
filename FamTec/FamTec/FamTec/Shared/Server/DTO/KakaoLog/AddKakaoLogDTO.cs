namespace FamTec.Shared.Server.DTO.KakaoLog
{
    public class AddKakaoLogDTO
    {
        /// <summary>
        /// 전송결과 코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 메시지 ID번호
        /// </summary>
        public string? MSGID { get; set; }

        /// <summary>
        /// 전송결과 메시지
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Phone { get; set; }
    }
}
