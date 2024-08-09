namespace FamTec.Shared.Server.DTO.Room
{
    public class RoomManagementDTO
    {
        /// <summary>
        /// 공간 인덱스
        /// </summary>
        public int? RoomId { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 건물명칭
        /// </summary>
        public string? BuilidingName { get; set; }

        /// <summary>
        /// 층명칭
        /// </summary>
        public string? FloorName { get; set; }

        /// <summary>
        /// 공간 생성일자
        /// </summary>
        public DateTime? RoomCreateDT { get; set; }
    }
}
