using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.Detail
{
    public class DetailMaterialListDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Name { get; set; }

        /// <summary>
        /// 해당 품목 입고수량 총합
        /// </summary>
        public int TotalInputNum { get; set; }

        /// <summary>
        /// 해당 품목 입고단가 총합
        /// </summary>
        public float TotalInputUnitPrice { get; set; }

        /// <summary>
        /// 해당 품목 입고 금액 총합
        /// </summary>
        public double TotalInputPrice { get; set; }

        // ==============================

        /// <summary>
        /// 해당 품목 출고수량 총합
        /// </summary>
        public int TotalOutputNum { get; set; }

        /// <summary>
        /// 해당 품목 출고 금액 총합
        /// </summary>
        public float TotalOutputUnitPrice { get; set; }

        /// <summary>
        /// 해당 품목 출고 금액 총합
        /// </summary>stock quantity
        public double TotalOutputPrice { get; set; }

        /// <summary>
        /// 총 재고 수량
        /// </summary>
        public int TotalStockNum { get; set; }

        /// <summary>
        /// 이월재고 수량
        /// </summary>
        public int LastMonthStock { get; set; }


        public List<InventoryRecordDTO> InventoryList { get; set; } = new List<InventoryRecordDTO>();

    }
}
