namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class UpdateGroupDTO
    {
        /// <summary>
        /// 그룹 인덱스
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 변경할 그룹명칭
        /// </summary>
        public string? GroupName { get; set; }
    }
}
