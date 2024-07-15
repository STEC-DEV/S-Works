using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class AddPlaceDTO
    {
        private DateTime _contractDt;
        public string? PlaceCd { get; set; }
        public string? Name { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public string? ContractNum { get; set; }
        public DateTime ContractDt { get; set; }
        
        //public DateTime ContractDt
        //{
        //    get { return _contractDt; }
        //    set { _contractDt = value.Date; } // Date 프로퍼티를 사용하여 시간 부분을 제거
        //}

        public bool PermMachine { get; set; } = false;
        public bool PermElec { get; set; } = false;
        public bool PermLift { get; set; } = false;
        public bool PermFire { get; set; } = false;
        public bool PermConstruct{ get; set; } = false;
        public bool PermNetwork{ get; set; } = false;
        public bool PermBeauty { get; set; } = false;
        public bool PermSecurity { get; set; } = false;
        public bool PermMaterial { get; set; } = false;
        public bool PermEnergy { get; set; } = false;
        public bool PermVoc { get; set; } = false;
    }
}
