using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Place
{
    public class PlacePerm
    {
        public int Id { get; set; }

        [Display(Name = "기계설비 권한")]
        public sbyte? PermMachine { get; set; } = 0;

        [Display(Name = "승강설비 권한")]
        public sbyte? PermLift { get; set; } = 0;

        [Display(Name = "소방설비 권한")]
        public sbyte? PermFire { get; set; } = 0;

        [Display(Name = "건축설비 권한")]
        public sbyte? PermConstruct { get; set; } = 0;

        [Display(Name = "통신설비 권한")]
        public sbyte? PermNetwork { get; set; } = 0;

        [Display(Name = "미화설비 권한")]
        public sbyte? PermBeauty { get; set; } = 0;

        [Display(Name = "보안설비 권한")]
        public sbyte? PermSecurity { get; set; } = 0;

        [Display(Name = "자재 권한")]
        public sbyte? PermMaterial { get; set; } = 0;

        [Display(Name = "에너지 권한")]
        public sbyte? PermEnergy { get; set; } = 0;

        [Display(Name = "민원 권한")]
        public sbyte? PermVoc { get; set; } = 0;
    }
}
