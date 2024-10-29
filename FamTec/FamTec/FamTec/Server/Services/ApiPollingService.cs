using FamTec.Shared.Server.DTO.KakaoLog;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Services
{
    public class ApiPollingService : BackgroundService
    {
        private readonly HttpClient HttpClient;
        private HttpResponseMessage? HttpResponse;
        
        private readonly GlobalStateService GlobalStateService;
        private readonly ConsoleLogService<ApiPollingService> CreateBuilderLogger;

        public ApiPollingService(HttpClient _httpClient,
            GlobalStateService _globalstateservice,
            ConsoleLogService<ApiPollingService> _createbuilderlogger)
        {
            this.HttpClient = _httpClient;
            this.GlobalStateService = _globalstateservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

#if DEBUG
            CreateBuilderLogger.ConsoleText("백그라운드 타이머 시작 / 간격 5분");
#endif
            while (!stoppingToken.IsCancellationRequested)
            {
                await RequestApi();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // 5분 대기
            }
        }

        private async Task RequestApi()
        {
            try
            {
                List<AddKaKaoSendResult> TargetMID = GlobalStateService.GetMIDList();
                

                foreach (AddKaKaoSendResult AddResult in TargetMID)
                {
                    var Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"apikey", Common.KakaoAPIKey },
                        { "userid", Common.KakaoUserId },
                        { "mid", AddResult.MID }
                    });

                    HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/history/detail/", Content).ConfigureAwait(false);
                    string HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    JObject? Jobj = JObject.Parse(HttpResponseResult);
                    string? code = (string?)Jobj["code"] ?? null;

                    // list 배열에서 첫 번째 객체 추출 (안전하게 접근)
                    var firstListItem = Jobj["list"]?.FirstOrDefault();

                    // 각 값 추출 (널 체크 포함)
                    string message = (string?)firstListItem?["rslt_message"] ?? null; // 실패사유
                    string msgid = (string?)firstListItem?["msgid"] ?? null; // 메시지ID
                    string phone = (string?)firstListItem?["phone"] ?? null; // 수신자

                    // DB INSERT

                }
             

               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
