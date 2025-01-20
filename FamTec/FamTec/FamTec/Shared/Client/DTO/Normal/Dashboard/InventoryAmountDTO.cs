using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Dashboard
{
    public class InventoryAmountDTO
    {
        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 총 수량
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// List
        /// </summary>
        public List<InventoryRoomDTO> RoomInvenList { get; set; } = new List<InventoryRoomDTO>();
    }

    public class InventoryRoomDTO
    {
        /// <summary>
        /// 창고명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }
    }
}
