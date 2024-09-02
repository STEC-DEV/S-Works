using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Location.Inventory
{
    public class MaterialHistoryDTO
    {
        /// <summary>
      /// 품목 ID
      /// </summary>
      public int? ID { get; set; }

        /// <summary>
        /// 품목 ID
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; } 

      /// <summary>
      /// 창고
      /// </summary>
      public List<RoomDTO>? RoomHistory { get; set; } = new List<RoomDTO>();
    }
}
