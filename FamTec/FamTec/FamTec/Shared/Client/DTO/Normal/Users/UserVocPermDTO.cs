using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class UserVocPermDTO
    {

        public bool MachineVoc{get; set;}
        public bool ElecVoc{get; set;}
        public bool LiftVoc {get; set;}
        public bool FireVoc {get; set;}
        public bool ConstructVoc {get; set;}
        public bool NetworkVoc {get; set;}
        public bool BeautyVoc {get; set;}
        public bool SecurityVoc {get; set;}

    }
}
