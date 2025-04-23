namespace FamTec.Server.Services
{
    public class CommService : ICommService
    {
        private readonly ILogService LogService;
        private readonly ConsoleLogService<CommService> CreateBuilderLogger;
        private readonly IHttpContextAccessor HttpContextAccessor;

        public CommService(ILogService _logservice,
            IHttpContextAccessor _httpcontextaccessor,
            ConsoleLogService<CommService> _createbuilderlogger)
        {
            this.LogService = _logservice;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        public string getRemoveWhiteSpace(string str)
        {
            try
            {
                return string.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 모바일 접속여부
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public bool MobileConnectCheck()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                // 모바일 여부
                bool isMobile = false;

                string? userAgent = context.Request.Headers["User-Agent"].ToString();
                if (String.IsNullOrWhiteSpace(userAgent))
                    return false;

#if DEBUG
                CreateBuilderLogger.ConsoleText(userAgent);
#endif

                if(userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== Mobile 접속 ====");
#endif
                    isMobile = true;
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC 접속 ====");
#endif
                    isMobile = false;
                }

                return isMobile;

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }


    }
}
