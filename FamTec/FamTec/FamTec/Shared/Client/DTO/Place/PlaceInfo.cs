using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class PlaceInfo
    {
        public int Id { get; set; }
        [Display(Name = "사업장 코드")]
        public string? PlaceCd { get; set; }
        [Display(Name = "사업장명")]
        public string? Name { get; set; }
        [Display(Name = "전화번호")]
        public string? Tel { get; set; }

        [Display(Name = "계약번호")]
        public string? ContractNum { get; set; }
        //주소
        public string? Address { get; set; }
        [Display(Name = "계약일자")]
        public DateTime? ContractDt { get; set; }
        [Display(Name = "해약일자")]
        public DateTime? CancelDt { get; set; }
        [Display(Name="계약상태")]
        public bool? Status{ get; set; }
        [Display(Name = "비고")]
        public string? Note { get; set; }

        public PlaceInfo()
        {
        }

        // 복사 생성자 추가
        public PlaceInfo(PlaceInfo source)
        {
            if (source != null)
            {
                Id = source.Id;
                PlaceCd = source.PlaceCd;
                Name = source.Name;
                Address = source.Address;
                Tel = source.Tel;
                ContractNum = source.ContractNum;
                ContractDt = source.ContractDt;
                CancelDt = source.CancelDt;
                Note = source.Note;
                Status = source.Status;
            }
        }

        // 깊은 복사 메서드 추가
        public PlaceInfo DeepCopy()
        {
            return new PlaceInfo(this);
        }
    }
}
