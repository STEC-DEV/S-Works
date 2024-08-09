namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class AddKeyDTO
    {
        /// <summary>
        /// 상위 그룹ID
        /// </summary>
        public int? GroupID { get; set; }

        /// <summary>
        /// 키 명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }
        
        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();
    }
}
