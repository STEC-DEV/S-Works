namespace FamTec.Shared.Server.DTO.User
{
    /// <summary>
    /// 사업장 소속 전체 LIST 관리자포함.
    /// </summary>
    public class PlaceUserDTO
    {
        /// <summary>
        /// 유저테이블 인덱스
        /// </summary>
        public int userIdx { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? userName { get; set; }
    }
}
