using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class AddManagerPlaceDTO
    {
        public int AdminId { get; set; }
        public List<int> PlaceList { get; set; } = new List<int>();
    }
}
