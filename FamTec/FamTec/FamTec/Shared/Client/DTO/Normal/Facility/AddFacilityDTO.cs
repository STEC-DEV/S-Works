using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility
{
    public class AddFacilityDTO
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Num { get; set; }

        public string Unit { get; set; }
        public DateTime? EquipDT { get; set; }
        public string LifeSpan { get; set; }
        public string StandardCapacity {get;set;}
        public DateTime? ChangeDT { get; set; }
        public int RoomTbId { get; set; }
    }
}
