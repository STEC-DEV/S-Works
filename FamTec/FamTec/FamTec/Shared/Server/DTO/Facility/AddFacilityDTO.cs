using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Facility
{
    /// <summary>
    /// 설비 추가 DTO
    /// </summary>
    public class AddFacilityDTO
    {
        /// <summary>
        /// 설비 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 카테고리(기계, 전기...)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 설비명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 개수
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 형식
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 규격용량
        /// </summary>
        public string? Standard_capacity { get; set; }

        /// <summary>
        /// 설치년월
        /// </summary>
        public DateTime? FacCreateDT { get; set; }

        /// <summary>
        /// 내용연수
        /// </summary>
        public string? LifeSpan { get; set; }

        /// <summary>
        /// 규격용량단위
        /// </summary>
        public string? Standard_unit { get; set; }

        /// <summary>
        /// 교체년월
        /// </summary>
        public DateTime? ChangeDT { get; set; }
        
        /// <summary>
        /// 공간 인덱스
        /// </summary>
        public int? RoomTbId { get; set; }
    }
}
