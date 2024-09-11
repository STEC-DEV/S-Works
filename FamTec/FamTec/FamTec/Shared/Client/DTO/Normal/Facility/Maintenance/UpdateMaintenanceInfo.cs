using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class UpdateMaintenanceInfo
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 작업명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
    }
}
