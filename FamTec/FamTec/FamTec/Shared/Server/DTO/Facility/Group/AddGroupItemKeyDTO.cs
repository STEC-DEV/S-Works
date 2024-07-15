namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class AddGroupItemKeyDTO
    {
        /// <summary>
        /// 키 명칭
        /// </summary>
        public string? Name { get; set; }

        public List<AddGroupItemValueDTO> ItemValues { get; set; } = new List<AddGroupItemValueDTO>();
    }
}
