namespace FamTec.Shared.Server.DTO.Building.Group.Key
{
    public class UpdateKeyDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 키 명칭
        /// </summary>
        public string? Itemkey { get; set; }

        /// <summary>
        /// 단위명
        /// </summary>
        public string? Unit { get; set; }

        public List<GroupValueListDTO>? ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
