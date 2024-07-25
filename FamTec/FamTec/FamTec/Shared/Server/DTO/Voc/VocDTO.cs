using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Voc
{
    public class VocDTO
    {
        /// <summary>
        /// VOC ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 위치
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 유형
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 작성자
        /// </summary>
        public string? Writer { get; set; } 

        /// <summary>
        /// 처리상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 민원 발생시간
        /// </summary>
        public DateTime? CreateDT { get; set; }
        
        /// <summary>
        /// 민원처리 완료 시간
        /// </summary>
        public DateTime? CompleteDT { get; set; }

        /// <summary>
        /// 민원처리 소요 시간
        /// </summary>
        public DateTime? DurationDT { get; set; }
    }
}
