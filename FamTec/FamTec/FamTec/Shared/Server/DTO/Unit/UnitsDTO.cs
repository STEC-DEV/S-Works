namespace FamTec.Shared.Server.DTO.Unit
{
    public class UnitsDTO
    {
        /// <summary>
        /// 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 단위명
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 시스템 생성여부
        /// </summary>
        public bool SystemCreate { get; set; }
    }
}
