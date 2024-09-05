using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class UseStoreDTO
    {
        public int StoreID { get; set; }

        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 품목 Code
        /// </summary>
        public string? MaterialCode { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacuringComp { get; set; }

        /// <summary>
        /// 출고창고 ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 출고창고 명
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 출고금액
        /// </summary>
        public float TotalPrice { get; set; }

    }
}
