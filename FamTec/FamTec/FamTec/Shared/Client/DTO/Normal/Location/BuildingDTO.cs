using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location
{
    public class BuildingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FloorDTO> Floors { get; set; } = new List<FloorDTO>();
    }
}
