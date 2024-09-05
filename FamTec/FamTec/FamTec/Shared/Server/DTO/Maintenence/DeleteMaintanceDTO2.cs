namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 유지보수 이력 1건에 대한 전체삭제
    /// </summary>
    public class DeleteMaintanceDTO2
    {
        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 유지보수 인덱스
        /// </summary>
        public List<int> MaintanceID { get; set; } = new List<int>();
    }
}
