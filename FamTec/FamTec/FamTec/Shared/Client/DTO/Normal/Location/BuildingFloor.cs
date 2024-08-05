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
        public int? FloorId { get; set; }

        /// <summary>
        /// 층 명
        /// </summary>
        public string? Name { get; set; }

    }
}
