using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class PlacePerm
    {
        public int Id { get; set; }

        [Display(Name = "기계설비 권한")]
        public bool PermMachine { get; set; }
        [Display(Name = "전기설비 권한")]
        public bool PermElec { get; set; }

        [Display(Name = "승강설비 권한")]
        public bool PermLift { get; set; }

        [Display(Name = "소방설비 권한")]
        public bool PermFire { get; set; }

        [Display(Name = "건축설비 권한")]
        public bool PermConstruct { get; set; }

        [Display(Name = "통신설비 권한")]
        public bool PermNetwork { get; set; }

        [Display(Name = "미화설비 권한")]
        public bool PermBeauty { get; set; }

        [Display(Name = "보안설비 권한")]
        public bool PermSecurity { get; set; }

        [Display(Name = "자재 권한")]
        public bool PermMaterial { get; set; }

        [Display(Name = "에너지 권한")]
        public bool PermEnergy { get; set; }

        [Display(Name = "민원 권한")]
        public bool PermVoc { get; set; }

        // 기본 생성자
        public PlacePerm()
        {
        }

        // 복사 생성자
        public PlacePerm(PlacePerm source)
        {
            if (source != null)
            {
                Id = source.Id;
                PermMachine = source.PermMachine;
                PermElec = source.PermElec;
                PermLift = source.PermLift;
                PermFire = source.PermFire;
                PermConstruct = source.PermConstruct;
                PermNetwork = source.PermNetwork;
                PermBeauty = source.PermBeauty;
                PermSecurity = source.PermSecurity;
                PermMaterial = source.PermMaterial;
                PermEnergy = source.PermEnergy;
                PermVoc = source.PermVoc;
            }
        }

        // 깊은 복사 메서드
        public PlacePerm DeepCopy()
        {
            return new PlacePerm(this);
        }

    }
}

