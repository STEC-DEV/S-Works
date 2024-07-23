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

        public AddStoreDTO AddStore { get; set; } = new AddStoreDTO();
    }
}
