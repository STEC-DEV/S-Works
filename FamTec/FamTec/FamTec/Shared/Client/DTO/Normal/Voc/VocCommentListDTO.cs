using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class VocCommentListDTO
    {
        /// <summary>
        /// 민원 댓글 ID
        /// </summary>
        public int? Id { get; set; }

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
        /// 민원 댓글 상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// VOC 인덱스
        /// </summary>
        public int? VocTbId { get; set; }

    }
}
