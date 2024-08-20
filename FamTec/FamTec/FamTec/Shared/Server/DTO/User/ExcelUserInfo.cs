namespace FamTec.Shared.Server.DTO.User
{
    /// <summary>
    /// 엑셀 Import DATA
    /// </summary>
    public class ExcelUserInfo
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string? UserID { get; set; }

        /// <summary>
        /// 사용자 비밀번호
        /// </summary>
        public string? PassWord { get; set; }

        /// <summary>
        /// 사용자 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 사용자 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 사용자 전화번호
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}
