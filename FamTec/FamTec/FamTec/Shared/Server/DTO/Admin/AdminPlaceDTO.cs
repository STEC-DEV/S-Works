namespace FamTec.Shared.Server.DTO.Admin
{
    public class AdminPlaceDTO
    {
        /// <summary>
        /// PlaceID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 사업장코드
        /// </summary>
        public string? PlaceCd { get; set; }

        /// <summary>
        /// 사업장명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        public string? ContractNum { get; set; } = null;

        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDt { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        public bool? Status { get; set; }

    }
}
