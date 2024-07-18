using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Place
{
    public class PlaceInfo
    {
        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 사업장코드
        /// </summary>
        [Display(Name = "사업장 코드")]
        public string? PlaceCd { get; set; }

        /// <summary>
        /// 사업장명
        /// </summary>
        [Display(Name = "사업장명")]
        public string? Name { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        [Display(Name = "전화번호")]
        public string? Tel { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        [Display(Name = "계약번호")]
        public string? ContractNum { get; set; }

        /// <summary>
        /// 계약일자
        /// </summary>
        [Display(Name = "계약일자")]
        public DateTime? ContractDt { get; set; }

        /// <summary>
        /// 해약일자
        /// </summary>
        [Display(Name = "해약일자")]
        public DateTime? CancelDt { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        [Display(Name = "계약상태")]
        public bool? Status { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        [Display(Name = "비고")]
        public string? Note { get; set; }
    }
}
