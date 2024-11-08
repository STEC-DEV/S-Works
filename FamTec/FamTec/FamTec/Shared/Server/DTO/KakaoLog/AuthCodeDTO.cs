namespace FamTec.Shared.Server.DTO.KakaoLog
{
    /// <summary>
    /// 인증코드 DTO
    /// </summary>
    public class AuthCodeDTO
    {
        /// <summary>
        /// 사업장 ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 건물ID
        /// </summary>
        public int BuildingId { get; set; }

        /// <summary>
        /// 사용자이름
        /// </summary>
        //public string? UserName { get; set; }

        /// <summary>
        /// 사용자 전화번호
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}
