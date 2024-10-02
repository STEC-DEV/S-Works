using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.InOut
{
    public class InOutInventoryDTO
    {
        /// <summary>
        /// 입출고 구분
        /// </summary>
        public int? InOut { get; set; }

        /// <summary>
        /// 품목ID
        /// </summary>
        public int? MaterialID { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialCode { get; set; }
        public string? Unit { get; set; }
        public AddStoreDTO AddStore { get; set; } = new AddStoreDTO();
    }
}
