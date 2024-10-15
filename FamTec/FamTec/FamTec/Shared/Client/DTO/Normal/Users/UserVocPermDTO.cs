using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class UserVocPermDTO
    {

        public bool MachineVoc { get; set; } = false;
        public bool ElecVoc{get; set; } = false;
        public bool LiftVoc {get; set; } = false;
        public bool FireVoc {get; set; } = false;
        public bool ConstructVoc {get; set; } = false;
        public bool NetworkVoc {get; set; } = false;
        public bool BeautyVoc {get; set; } = false;
        public bool SecurityVoc { get; set; } = false;
        public bool ETCVoc { get; set; } = false;


        public UserVocPermDTO Clone()
        {
            return new UserVocPermDTO
            {
                MachineVoc = this.MachineVoc,
                ElecVoc = this.ElecVoc,
                LiftVoc = this.LiftVoc,
                FireVoc = this.FireVoc,
                ConstructVoc = this.ConstructVoc,
                NetworkVoc = this.NetworkVoc,
                BeautyVoc = this.BeautyVoc,
                SecurityVoc = this.SecurityVoc,
                ETCVoc = this.ETCVoc
            };
        }
    }
}
