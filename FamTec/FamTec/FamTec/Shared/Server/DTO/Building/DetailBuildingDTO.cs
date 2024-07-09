using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 상세정보 DTO
    /// </summary>
    public class DetailBuildingDTO
    {
        /// <summary>
        /// 건물 테이블 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 건물코드
        /// </summary>
        public string? BuildingCD { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 건물주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 건물용도
        /// </summary>
        public string? Usage { get; set; }

        /// <summary>
        /// 시공업체
        /// </summary>
        public string? ConstComp { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public DateTime? CompletionDT { get; set; }

        /// <summary>
        /// 건물구조
        /// </summary>
        public string? BuildingStruct { get; set; }

        /// <summary>
        /// 지붕구조
        /// </summary>
        public string? RoofStruct { get; set; }
        
        /// <summary>
        /// 첨부파일
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// 추가항목
        /// </summary>
        public List<GroupListDTO>? GroupItem { get; set; } = new List<GroupListDTO>();

    }
}
