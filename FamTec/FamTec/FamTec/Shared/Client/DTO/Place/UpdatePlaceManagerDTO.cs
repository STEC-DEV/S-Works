using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class UpdatePlaceManagerDTO
    {
        public int PlaceId { get; set; }
        public List<int> AdminId { get; set; } = new List<int>();
    }
}
