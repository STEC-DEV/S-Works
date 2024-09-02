namespace FamTec.Shared.Server.DTO.Store
{
    public class MaterialHistory
    {
        /// <summary>
        /// 품목 ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; } 

        /// <summary>
        /// 품목코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 창고
        /// </summary>
        public List<RoomDTO>? RoomHistory { get; set; } = new List<RoomDTO>();
    }
}
