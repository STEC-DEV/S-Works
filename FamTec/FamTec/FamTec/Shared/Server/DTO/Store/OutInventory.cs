namespace FamTec.Shared.Server.DTO.Store
{
    public class OutInventory
    {
        /// <summary>
        /// 자재 ID
        /// </summary>
        public int? materialid { get; set; }
        
        /// <summary>
        /// 공간ID
        /// </summary>
        public int? roomid { get; set; }

        /// <summary>
        /// 삭제할 개수
        /// </summary>
        public int? OutCount { get; set; } 
    }
}
