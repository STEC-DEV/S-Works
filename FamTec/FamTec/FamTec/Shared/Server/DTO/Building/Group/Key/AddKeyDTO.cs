namespace FamTec.Shared.Server.DTO.Building.Group.Key
{
    public class AddKeyDTO
    {
        /// <summary>
        /// 상위 그룹ID
        /// </summary>
        public int? GroupID { get; set; }
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();
    }
}
