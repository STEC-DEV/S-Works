namespace FamTec.Shared.Server.DTO.Floor
{
    /// <summary>
    /// 층수정 DTO
    /// </summary>
    public class UpdateFloorDTO
    {
        /// <summary>
        /// 층 INDEX
        /// </summary>
        public int FloorID { get; set; }

        /// <summary>
        /// 층 이름
        /// </summary>
        public string? Name { get; set; }
    }
}
