namespace FamTec.Shared.Server.DTO.Store
{
    public class AddStoreDTO
    {
        /// <summary>
        /// 입-출고 날짜
        /// </summary>
        public DateTime InOutDate { get; set; }

        /// <summary>
        /// 창고ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }

        /// <summary>
        /// 입-출고 수량
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 합계 가격
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

    }
}
