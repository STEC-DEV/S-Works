using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class UpdatePlaceDTO
    {
        public PlaceInfo PlaceInfo { get; set; } = new PlaceInfo();
        public PlacePerm PlacePerm { get; set; } = new PlacePerm();

    }
}
