namespace FamTec.Shared.Server.DTO.Room
{
    public class RoomListDTO
    {
        /// <summary>
        /// 공간 테이블 INDEX
        /// </summary>
        public int? RoomID { get; set; }

        /// <summary>
        /// 공간 명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 건물 인덱스
        /// </summary>
        public int? BuildingID { get; set; }

        /// <summary>
        /// 건물 이름
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 층 인덱스
        /// </summary>
        public int? FloorID { get; set; }

        /// <summary>
        /// 층 이름
        /// </summary>
        public string? FloorName { get; set; }

        /// <summary>
        /// 등록일자
        /// </summary>
        public DateTime? CreateDT { get; set; }
    }
}
