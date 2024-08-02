namespace FamTec.Shared.Server.DTO.Building.Group.Key
{
    public class UpdateKeyDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        public int? ID { get; set; }
        public string? Itemkey { get; set; }
        public string? Unit { get; set; }

        public List<GroupValueListDTO> ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
