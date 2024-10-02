using FamTec.Shared.Client.DTO.Normal.Location.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location
{
    public class BuildingFloor
    {
        /// <summary>
        /// 층 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 층 명
        /// </summary>
        public string? Name { get; set; }

        public List<BuildingRoomDTO>? RoomList { get; set; } = new List<BuildingRoomDTO>();
    }
}
