using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class AllPlaceDTO
    {
        /// <summary>
        /// 사업장ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        [Display(Name = "선택")]
        public bool? IsSelect { get; set; } = false;

        /// <summary>
        /// 사업장명
        /// </summary>
        [Display(Name = "사업장명")]
        public string? Name { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        [Display(Name = "계약번호")]
        public string? ContractNum { get; set; } = null;

        /// <summary>
        /// 계약일자
        /// </summary>
        [Display(Name = "계약일자")]
        public DateTime? ContractDt { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        [Display(Name = "계약상태")]
        public bool? Status { get; set; }

        [Display(Name = "해약일자")]
        public DateTime? CancelDt { get; set; }
        
    }
}
