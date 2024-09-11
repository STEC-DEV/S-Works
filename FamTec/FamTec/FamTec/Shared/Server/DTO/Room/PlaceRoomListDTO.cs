namespace FamTec.Shared.Server.DTO.Room
{
    public class PlaceRoomListDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 층 리스트
        /// </summary>
        public List<BuildingFloor> FloorList { get; set; } = new List<BuildingFloor>();

    }

    public class BuildingFloor
    {
        /// <summary>
        /// 층 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 층 명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 공간 리스트
        /// </summary>
        public List<FloorRoom> RoomList { get; set; } = new List<FloorRoom>();
    }

    public class FloorRoom
    {
        /// <summary>
        /// 공간ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? Name { get; set; }
    }
}
