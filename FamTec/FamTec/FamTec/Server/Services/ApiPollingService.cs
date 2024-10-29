using FamTec.Client.Pages.Voc.Select.Components;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FamTec.Server.Services
{
    public class ApiPollingService : BackgroundService
    {
        private readonly HttpClient HttpClient;
        private HttpResponseMessage? HttpResponse;
        private readonly GlobalStateService GlobalStateService;

        public ApiPollingService(HttpClient _httpClient,
            GlobalStateService _globalstateservice)
        {
            this.HttpClient = _httpClient;
            this.GlobalStateService = _globalstateservice;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("API Polling Service 시작됨"); ;

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
                List<string> TargetMid = GlobalStateService.GetMIDList();
                foreach(string mid in TargetMid)
                {
                    var Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"apikey", Common.KakaoAPIKey },
                        { "userid", Common.KakaoUserId },
                        { "mid", mid }
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


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
