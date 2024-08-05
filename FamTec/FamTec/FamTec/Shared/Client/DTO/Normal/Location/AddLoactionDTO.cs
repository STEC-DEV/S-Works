using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location
{
    public class AddLoactionDTO
    {
        public string Name { get; set; }
        public int BuildingId { get; set; }
        public int FloorId{ get; set; }

    }
}
