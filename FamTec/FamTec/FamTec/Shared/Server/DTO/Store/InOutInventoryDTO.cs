namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 입출고 등록
    /// </summary>
    public class InOutInventoryDTO
    {
        /// <summary>
        /// 입출고 구분
        /// </summary>
        public int? InOut { get; set; }

        /// <summary>
        /// 품목ID
        /// </summary>
        public int? MaterialID { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }
        
        /// <summary>
        /// 품목코드
        /// </summary>
        public string? MaterialCode { get; set; }
        
        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        public AddStoreDTO? AddStore { get; set; } = new AddStoreDTO();
    }
}
