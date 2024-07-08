using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class UserPermDTO
    {
        [Display(Name ="기본 정보")]
        public sbyte BasicPerm { get; set; }
        [Display(Name = "기계 설비")]
        public sbyte MachinPerm { get; set; }
        [Display(Name = "전기 설비")]
        public sbyte ElecPerm { get; set; }
        [Display(Name = "승강 설비")]
        public sbyte LiftPerm { get; set; }
        [Display(Name = "소방 설비")]
        public sbyte FirePerm { get; set; }
        [Display(Name = "건축 설비")]
        public sbyte ConstructPerm { get; set; }
        [Display(Name = "통신 설비")]
        public sbyte NetworkPerm { get; set; }
        [Display(Name = "미화 설비")]
        public sbyte BeautyPerm { get; set; }
        [Display(Name = "보안 설비")]
        public sbyte SecurityPerm { get; set; }
        [Display(Name = "자재 관리")]
        public sbyte MaterialPerm { get; set; }
        [Display(Name = "에너지 관리")]
        public sbyte EnergyPerm { get; set; }
        [Display(Name = "사용자 관리")]
        public sbyte UserPerm { get; set; }
        [Display(Name = "민원 관리")]
        public sbyte VocPerm { get; set; }


    }
}
