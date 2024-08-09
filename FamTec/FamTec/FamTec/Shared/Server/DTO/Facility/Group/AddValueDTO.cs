namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class AddValueDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        public int? KeyID { get; set; }

        /// <summary>
        /// 값 명칭
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 단위 명칭
        /// </summary>
        public string? Unit { get; set; }
    }
}
