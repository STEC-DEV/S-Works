using FamTec.Shared.Server.DTO.Admin.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Place
{
    /// <summary>
    /// 사업장에 관리자추가 DTO
    /// </summary>
    public class AddPlaceManagerDTO<T>
    {
        public int PlaceId { get; set; }
        public List<ManagerListDTO> PlaceManager { get; set; } = new List<ManagerListDTO>();
    }
}
