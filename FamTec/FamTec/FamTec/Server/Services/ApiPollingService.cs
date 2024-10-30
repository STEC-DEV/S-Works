using FamTec.Server.Databases;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace FamTec.Server.Services
{
    public class ApiPollingService : BackgroundService
    {
        private readonly HttpClient HttpClient;
        private HttpResponseMessage? HttpResponse;

        private readonly ILogService LogService;
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly ConsoleLogService<ApiPollingService> CreateBuilderLogger;

        public ApiPollingService(HttpClient _httpClient,
            IServiceScopeFactory _scopeFactory,
            ILogService _logservice,
            ConsoleLogService<ApiPollingService> _createbuilderlogger)
            {
                this.HttpClient = _httpClient;
                this.ScopeFactory = _scopeFactory;
                this.CreateBuilderLogger = _createbuilderlogger;
                this.LogService = _logservice;
            }

      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText("백그라운드 타이머 시작 / 간격 30분");
#endif
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RequestApi();
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // 30분 대기
                }
            }
            catch(Exception ex)
            {
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                LogService.LogMessage(ex.ToString());
            }
        }
        
        private async Task RequestApi()
        {
            try
            {
                using (var scope = ScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WorksContext>();

                    // PerformTask 함수 실행
                    await PerformTask(dbContext);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                LogService.LogMessage(ex.ToString());
            }
        }


        private async Task PerformTask(WorksContext dbContext)
        {
#if DEBUG
            CreateBuilderLogger.ConsoleText("타이머 이벤트 동작");
#endif

            IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
            bool updatePerformed = false;

            await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        List<KakaoLogTb>? LogList = await dbContext.KakaoLogTbs
                            .Where(m => m.DelYn != true && m.MsgUpdate != true && m.Msgid != null)
                            .ToListAsync();

                        if (LogList is [_, ..])
                        {
                            foreach (KakaoLogTb logTB in LogList)
                            {
                                if (!string.IsNullOrWhiteSpace(logTB.Msgid))
                                {
                                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                                    {
                                        { "apikey", Common.KakaoAPIKey },
                                        { "userid", Common.KakaoUserId },
                                        { "mid", logTB.Msgid }
                                    });

                                    using (var response = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/history/detail", content).ConfigureAwait(false))
                                    {
                                        string responseContent = await response.Content
                                            .ReadAsStringAsync()
                                            .ConfigureAwait(false);

                                        JObject? jobj = JObject.Parse(responseContent);
                                        
                                        string? msgid = (string?)jobj["list"]?.FirstOrDefault()?["msgid"];

                                        // 통신사결과 MSGID가 Q로 시작되면 아직 반환값이 안왔다는것임.
                                        // 최대 24시간이 걸림.
                                        if (!string.IsNullOrWhiteSpace(msgid) && !msgid.StartsWith("Q")) 
                                        {
                                            // 데이터 업데이트
                                            logTB.Code = jobj["code"]?.ToString();
                                            logTB.Rslt = (string?)jobj["list"]?.FirstOrDefault()?["rslt"];
                                            logTB.RsltMessage = (string?)jobj["list"]?.FirstOrDefault()?["rslt_message"];
                                            logTB.MsgUpdate = true;
                                            logTB.UpdateDt = DateTime.Now;
                                            logTB.UpdateUser = "AligoMessage";

                                            dbContext.Update(logTB);
                                            updatePerformed = true; // 업데이트 플래그 설정
                                        }
                                    }
                                }
                            }

                            if (updatePerformed)
                            {
                                // 업데이트된 경우에만 저장 및 커밋
                                bool updateResult = await dbContext.SaveChangesAsync().ConfigureAwait(false) > 0;
                                if (updateResult)
                                {
                                    await transaction.CommitAsync().ConfigureAwait(false);
                                }
                                else
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                // 업데이트가 없으면 트랜잭션 중단
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        LogService.LogMessage(ex.ToString());
                    }
                }
            });
        }

    }
}
