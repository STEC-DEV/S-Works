namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class GroupListDTO
    {
        /// <summary>
        /// 그룹 ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 그룹명칭
        /// </summary>
        public string? Name { get; set; }

        public List<GroupKeyListDTO>? KeyListDTO { get; set; } = new List<GroupKeyListDTO>();

    }
}
