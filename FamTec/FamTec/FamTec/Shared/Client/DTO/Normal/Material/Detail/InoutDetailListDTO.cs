using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.Detail
{
    public class InoutDetailListDTO
    {
        public string InoutDate { get; set; }
        public string MaterialCode { get; set;}
        public string MaterialName { get; set;}
        public string Unit { get; set; }
        public int InNum { get; set;}
        public int InUnitPrice { get; set; }
        public int InTotalPrice { get; set; }
        public int OutNum { get; set;}
        public int OutUnitPrice { get; set; }
        public int OutTotalUnitPrice { get; set;}
        public int InventroyNum { get; set; }
        public string Note { get; set; }
    }
}
