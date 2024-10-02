using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location
{
    public class BuildingFloorListDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 층 리스트
        /// </summary>
        public List<BuildingFloor> FloorList { get; set; } = new List<BuildingFloor>();

    }


}
