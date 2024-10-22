namespace FamTec.Shared.Server.DTO.KakaoLog
{
    public class KaKaoSenderResult
    {
        /// <summary>
        /// 보낸이
        /// </summary>
        public string? Sender { get; set; }

        /// <summary>
        /// 메시지 내용
        /// </summary>
        public string? Message { get; set; }
        

        public DateTime SendDate { get; set; }

        public string? Result { get; set; }

    }
}
