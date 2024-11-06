namespace FamTec.Server.Services
{
    public interface ICommService
    {
        /// <summary>
        /// 해당문자 공백제거
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string getRemoveWhiteSpace(string str);

        /// <summary>
        /// 모바일 접속여부
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public bool MobileConnectCheck(HttpContext context);
    }
}
