namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 입출고 LIST
    /// </summary>
    public class InOutHistoryListDTO
    {
        /// <summary>
        /// 입출고 테이블ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 입출고 구분
        /// </summary>
        public int? INOUT { get; set; }

        /// <summary>
        /// 입출고날짜
        /// </summary>
        public DateTime? InOutDate { get; set; }

        /// <summary>
        /// 공간 인덱스
        /// </summary>
        public int? RoomID { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 품목 인덱스
        /// </summary>
        public int? MaterialID { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 품목단위
        /// </summary>
        public string? MaterialUnit { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float? UnitPrice { get; set; }

        /// <summary>
        /// 금액
        /// </summary>
        public float? ToTalPrice { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }
    }
}
