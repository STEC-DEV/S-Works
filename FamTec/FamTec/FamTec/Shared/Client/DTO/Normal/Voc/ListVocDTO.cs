using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class ListVocDTO
    {
        /// <summary>
        /// VOC ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 유형
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string? Title { get; set; }


        /// <summary>
        /// 처리상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 민원처리 완료 시간
        /// </summary>
        public string? CompleteDT { get; set; }

        /// <summary>
        /// 민원처리 소요 시간
        /// </summary>
        public string? DurationDT { get; set; }

        /// <summary>
        /// 요청일시
        /// </summary>
        public string? CreateDT { get; set; }

        /// <summary>
        /// 모바일 여부 0 : 모바일 / 1 : 웹
        /// </summary>
        public int Division { get; set; }

        public string? CreateUser { get; set; }

    }
}
