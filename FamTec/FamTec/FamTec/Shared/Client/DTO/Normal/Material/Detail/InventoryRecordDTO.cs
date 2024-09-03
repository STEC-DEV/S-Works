using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.Detail
{
    public class InventoryRecordDTO
    {
        /// <summary>
        /// 거래일
        /// </summary>
        public DateTime? INOUT_DATE { get; set; }

        /// <summary>
        /// 입출고 구분
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 품목코드
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 품목코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? MaterialUnit { get; set; }

        /// <summary>
        /// 입출고 수량
        /// </summary>
        public int? InOutNum { get; set; }

        /// <summary>
        /// 입출고 단가
        /// </summary>
        public float? InOutUnitPrice { get; set; }

        /// <summary>
        /// 입출고 금액
        /// </summary>
        public float? InOutTotalPrice { get; set; }

        /// <summary>
        /// 시점 재고 수량
        /// </summary>
        public int? CurrentNum { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

    }
}
