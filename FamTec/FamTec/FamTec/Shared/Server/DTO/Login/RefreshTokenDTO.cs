namespace FamTec.Shared.Server.DTO.Login
{
    public class RefreshTokenDTO
    {
        /// <summary>
        /// 사업장 ID
        /// </summary>
        public int? placeid { get; set; }

        /// <summary>
        /// 유저 인덱스
        /// </summary>
        public int? useridx { get; set; }

        /// <summary>
        /// true : 관리자설정 / false : 일반모드
        /// </summary>
        public bool isAdmin { get; set; }
    }
}
