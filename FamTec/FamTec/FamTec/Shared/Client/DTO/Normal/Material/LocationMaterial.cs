using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material
{
    public class LocationMaterial
    {
        /// <summary>
        /// 공간ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 공간 명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 자재ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 해당 공간의 품목수량
        /// </summary>
        public int Num { get; set; }
    }
}
