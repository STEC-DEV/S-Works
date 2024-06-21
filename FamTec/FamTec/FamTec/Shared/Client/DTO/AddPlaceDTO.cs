using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class AddPlaceDTO
    {
        public string? PlaceCd { get; set; }
        public string? Name { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public string? ContractNum { get; set; }
        public string? ContractDt { get; set; }

        public sbyte? PermMachine { get; set; } = 0;
        public sbyte? PermLift { get; set; } = 0;
        public sbyte? PermFire { get; set; } = 0;
        public sbyte? PermConstruct{ get; set; } = 0;
        public sbyte? PermNetwork{ get; set; } = 0;
        public sbyte? PermBeauty { get; set; } = 0;
        public sbyte? PermSecurity { get; set; } = 0;
        public sbyte? PermMaterial { get; set; } = 0;
        public sbyte? PermEnergy { get; set; } = 0;
        public sbyte? PermVoc { get; set; } = 0;
    }
}
