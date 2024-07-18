namespace FamTec.Shared.Server.DTO.Store
{
    /// <summary>
    /// 입출고 등록
    /// </summary>
    public class AddStoreDTO
    {
        /// <summary>
        /// 품목ID
        /// </summary>
        public int? MaterialID { get; set; }

        public List<StoreDTO> StoreList { get; set; } = new List<StoreDTO>();

    }
}
