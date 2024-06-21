namespace FamTec.Server.Tokens
{
    public class AdminSettingModel
    {
        public int? UserIdx { get; set; }
        public string? UserName { get; set; }
        public int? AdminIdx { get; set; }
        public int? DepartmentIdx { get; set; }
        public string? DepartmentName { get; set; }
        public string? Jti { get; set; }
        public string? Role { get; set; }
        public string? Exp { get; set; }
        public string? iss { get; set; }
        public string? aud { get; set; }
    }
}
