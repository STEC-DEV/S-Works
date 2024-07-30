using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Voc
{
    public class AddVocCommentDTO
    {
        /// <summary>
        /// 댓글내용
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 민원 처리상태
        /// </summary>
        public int? Status { get; set; }

        ///// <summary>
        ///// 사용자 이미지 - 클라이언트 담는용
        ///// </summary>
        //public byte[]? Image { get; set; }

        /// <summary>
        /// VOC 테이블 인덱스
        /// </summary>
        public int? VocTbId { get; set; }
    }
}
