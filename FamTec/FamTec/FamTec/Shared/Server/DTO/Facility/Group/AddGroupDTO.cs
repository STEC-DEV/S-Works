namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class AddGroupDTO
    {
        /// <summary>
        /// 설비 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 설비명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public List<AddGroupItemKeyDTO>? AddGroupKey { get; set; } = new List<AddGroupItemKeyDTO>();

    }
}
