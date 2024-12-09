using FamTec.Server.Services;

namespace FamTec.Server.Middleware
{
    public class IPManageMiddleware
    {
        private readonly ILogService LogService;
        private readonly RequestDelegate Next;
        private readonly ConsoleLogService<IPManageMiddleware> CreateBuilderLogger;


        public IPManageMiddleware(RequestDelegate _next,
            ILogService _logservice,
            ConsoleLogService<IPManageMiddleware> _createbuilderlogger)
        {
            this.Next = _next;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try 
            {
                string? ClientIP = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                string? ConvertIP = String.Empty;
                if (String.IsNullOrWhiteSpace(ClientIP))
                {
                    ConvertIP = context.Connection.RemoteIpAddress?.ToString();
#if DEBUG
                    CreateBuilderLogger.ConsoleText($"*** 접속IP : {ConvertIP}");
#endif
                }
                else
                {
                    ConvertIP = ClientIP.Split(',').FirstOrDefault();
#if DEBUG
                    CreateBuilderLogger.ConsoleText($"*** 접속IP : {ConvertIP}");
#endif
                }

                // IP가 없을수 없는데 혹시나.
                if (String.IsNullOrWhiteSpace(ConvertIP))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                // 차단 LIST
                List<string> ListIP = new List<string>
                {
                    //"123.2.156.148"
                };

                if (ListIP.Contains(ConvertIP))
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleWarning($"==== 접근금지 IP : {ConvertIP}");
#endif
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                await Next(context);

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

    }
}
