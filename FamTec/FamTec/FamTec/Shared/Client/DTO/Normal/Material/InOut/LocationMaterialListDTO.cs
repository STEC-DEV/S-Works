using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.InOut
{
    public class LocationMaterialListDTO
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }    
        public int MaterialId { get; set; }
        public int Num { get; set; }
    }
}
