namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 기간별 입출고내역 DTO
    /// </summary>
    public class PeriodicInventoryRecordDTO
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
        public int? MaterialID { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }
        
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
