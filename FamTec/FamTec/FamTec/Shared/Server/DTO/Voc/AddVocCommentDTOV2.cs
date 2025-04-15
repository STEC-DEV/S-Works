namespace FamTec.Shared.Server.DTO.Voc
{
    public class AddVocCommentDTOV2
    {
        /// <summary>
        /// 댓글내용
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 민원 처리상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// VOC 테이블 인덱스
        /// </summary>
        public int? VocTbId { get; set; }

        /// <summary>
        /// 알림톡 발송한건지 유무
        /// </summary>
        public bool sendYn { get; set; } = false;
    }
}
