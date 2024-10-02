using FamTec.Shared.Server.DTO.Admin.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Place
{
    public class DeletePlaceManagerDTO
    {
        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? PlaceId { get; set; }

        /// <summary>
        /// 매니저 리스트
        /// </summary>
        public List<ManagerDTO>? PlaceManager { get; set; } = new List<ManagerDTO>();
    }
}
