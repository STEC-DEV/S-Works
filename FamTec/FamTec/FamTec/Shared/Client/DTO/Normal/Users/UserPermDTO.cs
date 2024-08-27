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
        public int BasicPerm { get; set; }
        [Display(Name = "기계 설비")]
        public int MachinPerm { get; set; }
        [Display(Name = "전기 설비")]
        public int ElecPerm { get; set; }
        [Display(Name = "승강 설비")]
        public int LiftPerm { get; set; }
        [Display(Name = "소방 설비")]
        public int FirePerm { get; set; }
        [Display(Name = "건축 설비")]
        public int ConstructPerm { get; set; }
        [Display(Name = "통신 설비")]
        public int NetworkPerm { get; set; }
        [Display(Name = "미화 설비")]
        public int BeautyPerm { get; set; }
        [Display(Name = "보안 설비")]
        public int SecurityPerm { get; set; }
        [Display(Name = "자재 관리")]
        public int MaterialPerm { get; set; }
        [Display(Name = "에너지 관리")]
        public int EnergyPerm { get; set; }
        [Display(Name = "사용자 관리")]
        public int UserPerm { get; set; }
        [Display(Name = "민원 관리")]
        public int VocPerm { get; set; }


        public UserPermDTO Clone()
        {
            return new UserPermDTO
            {
                BasicPerm = this.BasicPerm,
                MachinPerm = this.MachinPerm,
                ElecPerm = this.ElecPerm,
                LiftPerm = this.LiftPerm,
                FirePerm = this.FirePerm,
                ConstructPerm = this.ConstructPerm,
                NetworkPerm = this.NetworkPerm,
                BeautyPerm = this.BeautyPerm,
                SecurityPerm = this.SecurityPerm,
                MaterialPerm = this.MaterialPerm,
                EnergyPerm = this.EnergyPerm,
                UserPerm = this.UserPerm,
                VocPerm = this.VocPerm
            };
        }

    }
}
