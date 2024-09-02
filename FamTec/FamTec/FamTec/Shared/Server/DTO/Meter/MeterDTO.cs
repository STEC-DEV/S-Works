namespace FamTec.Shared.Server.DTO.Meter
{
    public class MeterDTO
    {
        /// <summary>
        /// 검침기 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 검침기 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 종류 (전기, 기계..)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 계약종류 ID
        /// </summary>
        public int? ContractId { get; set; }

        /// <summary>
        /// 계약종류 명
        /// </summary>
        public string? ContractName { get; set; }
    }
}
