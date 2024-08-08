using Microsoft.AspNetCore.Http;

namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// VOC 상세정보 보기
    /// </summary>
    public class VocCommentDetailDTO
    {
        /// <summary>
        /// VOC Comment 인덱스
        /// </summary>
        public int VocCommentId { get; set; }
        
        /// <summary>
        /// 내용
        /// </summary>
        public string Content { get; set; } 
        
        /// <summary>
        /// 처리상태
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 작성일
        /// </summary>
        public DateTime CreateDT { get; set; }

        /// <summary>
        /// 작성자
        /// </summary>
        public string CreateUser { get; set; }

    
        /// <summary>
        /// 이미지
        /// </summary>
        public List<byte[]>? Images { get; set; } = new List<byte[]>(); // 이미지
    }
}
