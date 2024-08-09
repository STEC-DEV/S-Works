namespace FamTec.Shared.Server.DTO.BlackList
{
    /// <summary>
    /// 블랙리스트 조회 DTO
    /// </summary>
    public class BlackListDTO
    {
        /// <summary>
        /// 블랙리스트 테이블 인덱스
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 휴대폰 번호
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}
