namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupItemKeyDTO
    {
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        public string? Name { get; set; } // 아이템이름

        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();

    }
}
