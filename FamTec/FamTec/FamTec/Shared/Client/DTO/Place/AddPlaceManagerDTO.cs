using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class AddPlaceManagerDTO<T>
    {
        public int? PlaceId { get; set;}
        public List<ManagerDTO> PlaceManager { get; set;} = new List<ManagerDTO>();
    }
}
