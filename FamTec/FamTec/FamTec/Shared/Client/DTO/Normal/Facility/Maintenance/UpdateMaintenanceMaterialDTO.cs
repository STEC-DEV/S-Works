using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class UpdateMaintenanceMaterialDTO
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int MaintanceID { get; set; }

        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 공간 ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 사용내역 ID
        /// </summary>
        public int UseMaintanceID { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>
        /// 총 금액
        /// </summary>
        public float TotalPrice { get; set; }
    }
}
