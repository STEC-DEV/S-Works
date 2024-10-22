namespace FamTec.Server.Services
{
    public class CommService : ICommService
    {
        private readonly ILogService LogService;
        private readonly ConsoleLogService<CommService> CreateBuilderLogger;

        public CommService(ILogService _logservice,
            ConsoleLogService<CommService> _createbuilderlogger)
        {
            this.LogService = _logservice;
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
    }
}
