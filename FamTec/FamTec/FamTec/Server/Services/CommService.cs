namespace FamTec.Server.Services
{
    public class CommService : ICommService
    {
        ILogService LogService;

        public CommService(ILogService _logservice)
        {
            this.LogService = _logservice;
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
                throw;
            }
        }
    }
}
