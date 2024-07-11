namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupDTO
    {
        /// <summary>
        /// 건물인덱스
        /// </summary>
        public int? BuildingIdx { get; set; }
        /// <summary>
        /// 명칭 ex) 주차장
        /// </summary>
        public string? Name { get; set; }

        public List<AddGroupItemKeyDTO> AddGroupKey { get; set; } = new List<AddGroupItemKeyDTO>();

    }
}
