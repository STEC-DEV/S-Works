namespace FamTec.Shared.Server.DTO.Meter
{
    public class AddMeterDTO
    {
        /// <summary>
        /// 검침기 명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 검침종류 (전기, 기계 ...)
        /// </summary>
        public string? Category { get; set; } 

        /// <summary>
        /// 검침기 종류 ID - 전기일 경우 ID 필수
        /// </summary>
        public int? ContractTbId { get; set; }
    }
}
