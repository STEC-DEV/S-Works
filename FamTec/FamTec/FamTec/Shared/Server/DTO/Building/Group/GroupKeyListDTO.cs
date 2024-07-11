namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class GroupKeyListDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 키의 명칭
        /// </summary>
        public string? ItemKey { get; set; }

        public List<GroupValueListDTO> ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
