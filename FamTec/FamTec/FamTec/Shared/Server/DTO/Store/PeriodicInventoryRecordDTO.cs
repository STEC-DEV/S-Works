namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 기간별 입출고내역 DTO
    /// </summary>
    public class PeriodicDTO
    {
        public int? ID { get; set; }

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

        public List<InventoryRecord> InventoryList { get; set; } = new List<InventoryRecord>();
    }

    public class InventoryRecord
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
        /// 품목인덱스
        /// </summary>
        public int? ID { get; set; }

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

        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int? MaintanceId { get; set; }

        /// <summary>
        /// 유지보수 이동 URL
        /// </summary>
        public string? Url { get; set; }
    }
}
