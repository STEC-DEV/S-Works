using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class PlaceTableDTO
    {
        public int Id { get; set; }
        [Display(Name = "선택")]
        public bool IsSelect { get; set; } = false;
        [Display(Name ="사업장 코드")]
        public string? PlaceCd {  get; set; }
        [Display(Name = "사업장명")]
        public string? Name { get; set; } = null;
        [Display(Name = "비고")]
        public string? Note { get; set; } = null;
        [Display(Name = "계약번호")]
        public string? ContractNum { get; set; } = null;
        [Display(Name = "계약일자")]
        public DateTime? ContractDt { get; set; }
        [Display(Name = "계약상태")]
        public sbyte? Status { get; set; }
    }
}
