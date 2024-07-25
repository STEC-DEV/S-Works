using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// 민원등록 - 민원인용
    /// </summary>
    public class AddVocDTO
    {
        /// <summary>
        /// 민원의 제목
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string? Contents { get; set; }

        /// <summary>
        /// 유형
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 작성자 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 휴대폰번호
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 답변수신 여부
        /// </summary>
        public bool? ReplyYN { get; set; }

        /// <summary>
        /// 빌딩 인덱스
        /// </summary>
        public int? buildingidx { get; set; }

        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? PlaceIdx { get; set; }
    }
}
