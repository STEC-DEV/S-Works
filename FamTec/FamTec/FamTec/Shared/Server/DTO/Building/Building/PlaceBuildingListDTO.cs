namespace FamTec.Shared.Server.DTO.Building.Building
{
    public class PlaceBuildingListDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        public int? Id { get; set; }

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
        public int? Id { get; set; }

        /// <summary>
        /// 층 명
        /// </summary>
        public string? Name { get; set; }
    }

}
