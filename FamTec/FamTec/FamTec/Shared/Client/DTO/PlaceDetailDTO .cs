using FamTec.Shared.Client.DTO.Place;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class PlaceDetailDTO
    {
        public PlaceInfo PlaceInfo { get; set; } = new PlaceInfo();
        public PlacePerm PlacePerm { get; set; } = new PlacePerm();

        public List<ManagerDTO> ManagerList { get; set; } = new List<ManagerDTO>();
    }
}
