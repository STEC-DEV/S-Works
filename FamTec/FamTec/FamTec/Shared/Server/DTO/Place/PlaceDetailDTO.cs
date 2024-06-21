using FamTec.Shared.Server.DTO.Admin.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Place
{
    public class PlaceDetailDTO
    {
        /// <summary>
        /// 사업장 정보
        /// </summary>
        public PlaceInfo PlaceInfo { get; set; } = new PlaceInfo();

        /// <summary>
        /// 사업장 권한
        /// </summary>
        public PlacePerm PlacePerm { get; set; } = new PlacePerm();

        /// <summary>
        /// 사업장 매니저 리스트
        /// </summary>
        public List<ManagerListDTO> ManagerList { get; set; } = new List<ManagerListDTO>();
    }
}
