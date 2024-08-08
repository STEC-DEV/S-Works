using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Room
{
    /// <summary>
    /// 공간 추가용 DTO
    /// </summary>
    public class RoomDTO
    {
        /// <summary>
        /// 공간이름
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 층 인덱스
        /// </summary>
        public int FloorID { get; set; }
    }
}
