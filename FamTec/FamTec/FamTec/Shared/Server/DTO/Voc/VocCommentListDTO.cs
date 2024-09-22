namespace FamTec.Shared.Server.DTO.Voc
{
    public class VocCommentListDTO
    {
        /// <summary>
        /// 민원 댓글 ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 댓글상태
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 민원 댓글 내용
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 민원 댓글 생성일자
        /// </summary>
        public string? CreateDT { get; set; }

        /// <summary>
        /// 민원 댓글 생성자
        /// </summary>
        public string? CreateUser { get; set; }

        /// <summary>
        /// VOC 인덱스
        /// </summary>
        public int? VocTbId { get; set; }

        /// <summary>
        /// 이미지 파일명
        /// </summary>
        public List<string?> ImageName { get; set; } = new List<string?>();

        /// <summary>
        /// 이미지 리스트
        /// </summary>
        public List<byte[]?> Images { get; set; } = new List<byte[]?>();
    }

    
}
