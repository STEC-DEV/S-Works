namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 입출고 등록
    /// </summary>
    public class AddInventoryDTO
    {
        /// <summary>
        /// 품목ID
        /// </summary>
        public int? MaterialID { get; set; }

        public List<InventoryDTO> StoreList { get; set; } = new List<InventoryDTO>();

    }
}
