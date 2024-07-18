using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Store
{
    public class StoreDTO
    {
        /// <summary>
        /// 창고ID
        /// </summary>
        public int? RoomID { get; set; }


        public int? InOut { get; set; }

        /// <summary>
        /// 입-출고 수량
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public int? UnitPrice { get; set; }

        /// <summary>
        /// 입-출고 가격
        /// </summary>
        public int? TotalPrice { get; set; }

        /// <summary>
        /// 입-출고 날짜
        /// </summary>
        public DateTime? InOutDate { get; set; }
    }
}
