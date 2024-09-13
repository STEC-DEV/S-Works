using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location
{
    public class PlaceRoomListDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 층 리스트
        /// </summary>
        public List<PlaceFloor> FloorList { get; set; } = new List<PlaceFloor>();

    }

    public class PlaceFloor
    {
        /// <summary>
        /// 층 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 층 명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 공간 리스트
        /// </summary>
        public List<PlaceRoom> RoomList { get; set; } = new List<PlaceRoom>();
    }

    public class PlaceRoom
    {
        /// <summary>
        /// 공간ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? Name { get; set; }
    }
}
