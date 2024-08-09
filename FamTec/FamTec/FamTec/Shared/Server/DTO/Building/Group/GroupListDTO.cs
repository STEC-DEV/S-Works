namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class GroupListDTO
    {
        /// <summary>
        /// 그룹 ListID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 그룹명
        /// </summary>
        public string? Name { get; set; }


        public List<GroupKeyListDTO>? KeyListDTO { get; set; } = new List<GroupKeyListDTO>();
    }
}
