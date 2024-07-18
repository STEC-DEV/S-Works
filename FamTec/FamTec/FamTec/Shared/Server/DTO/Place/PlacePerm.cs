using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Place
{
    public class PlacePerm
    {
        public int Id { get; set; }

        [Display(Name = "기계설비 권한")]
        public bool? PermMachine { get; set; } = false;

        [Display(Name = "전기설비 권한")]
        public bool? PermElec { get; set; } = false;

        [Display(Name = "승강설비 권한")]
        public bool? PermLift { get; set; } = false;

        [Display(Name = "소방설비 권한")]
        public bool? PermFire { get; set; } = false;

        [Display(Name = "건축설비 권한")]
        public bool? PermConstruct { get; set; } = false;

        [Display(Name = "통신설비 권한")]
        public bool? PermNetwork { get; set; } = false;

        [Display(Name = "미화설비 권한")]
        public bool? PermBeauty { get; set; } = false;

        [Display(Name = "보안설비 권한")]
        public bool? PermSecurity { get; set; } = false;

        [Display(Name = "자재 권한")]
        public bool? PermMaterial { get; set; } = false;

        [Display(Name = "에너지 권한")]
        public bool? PermEnergy { get; set; } = false;

        [Display(Name = "민원 권한")]
        public bool? PermVoc { get; set; } = false;
    }
}
