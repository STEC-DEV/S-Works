using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class DeleteMaintenanceDTO
    {
        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 유지보수 인덱스
        /// </summary>
        public List<int> MaintanceID { get; set; } = new List<int>();
    }
}
