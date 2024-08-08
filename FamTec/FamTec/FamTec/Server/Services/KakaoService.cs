using FamTec.Shared.Server.DTO;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace FamTec.Server.Services
{
    public class KakaoService : IKakaoService
    {
        private FormUrlEncodedContent? Content;
        private HttpResponseMessage? HttpResponse;

        private string? HttpResponseResult = String.Empty;

        private readonly ILogService LogService;

        public KakaoService(ILogService _logservice)
        {
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원인 전용 VOC 등록 확인메시지
        /// </summary>
        /// <returns></returns>
        public async Task<KakaoLogDTO?> AddVocAnswer(string title, string receiptnum, DateTime receiptdate, string receiver, string url, string placetel)
        {
            try
            {
                // [1]. 토큰생성
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"apikey", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId }
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/token/create/30/s/", Content);
                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync();

                JObject? Jobj = JObject.Parse(HttpResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (String.IsNullOrWhiteSpace(token))
                    return null;

                string year = receiptdate.ToString("yyyy"); //년
                string month = receiptdate.ToString("MM"); // 월
                string day = receiptdate.ToString("dd"); // 일

                string message = $"민원이 접수되었습니다.\n■제목: {title}\n■접수번호: {receiptnum}\n■접수일: {year}년{month}월{day}일\n■문의전화: {placetel}";

                JObject buttonValue = new JObject();
                buttonValue.Add("name", "열람하기");
                buttonValue.Add("linkType", "WL");
                buttonValue.Add("linkTypeName", "웹 링크");
                buttonValue.Add("linkMo", url);
                buttonValue.Add("linkPc", url);

                JArray Jarr = new JArray();
                Jarr.Add(buttonValue);
                JObject buttonResult = new JObject();
                buttonResult.Add("button", Jarr);

                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId},
                    { "token",token},
                    { "senderkey",Common.KakaoSenderKey},
                    { "tpl_code",Common.KakaoTemplateCode_1},
                    { "sender",Common.KakaoSenders},
                    { "receiver_1",receiver},
                    { "subject_1",title},
                    { "message_1",message},
                    { "button_1",buttonResult.ToString()}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/alimtalk/send/", Content);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };


                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync();
                Jobj = JObject.Parse(HttpResponseResult);
                
                KakaoLogDTO LogDTO = new KakaoLogDTO();
                LogDTO.Code = Convert.ToString(Jobj["code"]);
                LogDTO.Message = Convert.ToString(Jobj["message"]);
                return LogDTO;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 민원인 전용 VOC 상태변경 메시지
        /// </summary>
        /// <param name="receiptnum">접수번호</param>
        /// <param name="status">진행상태</param>
        /// <param name="receiver">받는사람 전화번호</param>
        /// <param name="url">링크 URL</param>
        /// <param name="placetel">상버장 전화번호</param>
        /// <returns></returns>
        public async Task<KakaoLogDTO?> UpdateVocAnswer(string receiptnum, string status, string receiver, string url, string placetel)
        {
            try
            {
                // [1]. 토큰생성
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/token/create/30/s/", Content);
                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync();

                JObject? Jobj = JObject.Parse(HttpResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (String.IsNullOrWhiteSpace(token))
                    return null;

                string message = $" ■{receiptnum}의 진행사항이 변경되었습니다.\n■ 진행상태: {status}\n■ 문의전화: {placetel}";

                JObject buttonValue = new JObject();
                buttonValue.Add("name", "열람하기");
                buttonValue.Add("linkType", "WL");
                buttonValue.Add("linkTypeName", "웹 링크");
                buttonValue.Add("linkMo", url);
                buttonValue.Add("linkPc", url);

                JArray Jarr = new JArray();
                Jarr.Add(buttonValue);
                JObject buttonResult = new JObject();
                buttonResult.Add("button", Jarr);

                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId},
                    { "token", token},
                    { "senderkey", Common.KakaoSenderKey},
                    { "tpl_code",Common.KakaoTemplateCode_2},
                    { "sender", Common.KakaoSenders},
                    { "receiver_1",receiver},
                    { "subject_1","진행사항 변경"},
                    { "message_1",message},
                    { "button_1",buttonResult.ToString()}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/alimtalk/send/", Content);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync();
                Jobj = JObject.Parse(HttpResponseResult);

                KakaoLogDTO LogDTO = new KakaoLogDTO();
                LogDTO.Code = Convert.ToString(Jobj["code"]);
                LogDTO.Message = Convert.ToString(Jobj["message"]);
                return LogDTO;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// 랜덤코드 생성
        /// </summary>
        /// <returns></returns>
        public string RandomCode()
        {
            RandomGenerator generator = new RandomGenerator();
            string randomPassword = generator.RandomPassword();
            
            return randomPassword;
        }

        private class RandomGenerator
        {
            private readonly Random _random = new Random();

            private int RandomNumber(int min, int max)
            {
                return _random.Next(min, max);
            }

            private string RandomString(int size, bool lowerCase = false)
            {
                var builder = new StringBuilder(size);

                char offset = lowerCase ? 'a' : 'A';
                const int lettersOffset = 26;

                for (int i = 0; i < size; i++)
                {
                    var @char = (char)_random.Next(offset, offset + lettersOffset);
                    builder.Append(@char);
                }

                return lowerCase ? builder.ToString().ToLower() : builder.ToString();
            }


            public string RandomPassword()
            {
                var passwordBuilder = new StringBuilder();

                // 4글자의 소문자
                passwordBuilder.Append(RandomString(4));

                // 1000에서 9999 사이의 4자리 숫자
                passwordBuilder.Append(RandomNumber(1000, 9999));

                // 2글자의 대문자
                passwordBuilder.Append(RandomString(2));
                return passwordBuilder.ToString();
            }
        }

    }
}
