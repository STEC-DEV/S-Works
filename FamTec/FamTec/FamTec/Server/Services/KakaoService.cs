using FamTec.Server.Repository.KakaoLog;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace FamTec.Server.Services
{
    public class KakaoService : IKakaoService
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private FormUrlEncodedContent? Content;
        private string? HttpResponseResult = String.Empty;

        private readonly ILogService LogService;
        private readonly ConsoleLogService<KakaoService> CreateBuilderLogger;

        public KakaoService(ILogService _logservice,
            IHttpClientFactory _httpclientfactory,
            ConsoleLogService<KakaoService> _createbuilderlogger)
        {
            this.LogService = _logservice;
            this.HttpClientFactory = _httpclientfactory;
            this.CreateBuilderLogger = _createbuilderlogger;
        }


        // 카카오 메시지결과 테스트
        public async Task<ResponseList<KaKaoSenderResult>?> KakaoSenderResult(HttpContext context,int page, int pagesize, DateTime StartDate, int limit_day)
        {
            try
            {
                if (context is null)
                    return new ResponseList<KaKaoSenderResult>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // [1]. 토큰생성
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"key", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId },
                    { "page",page.ToString()},
                    { "page_size",pagesize.ToString()},
                    { "start_date",StartDate.ToString("yyyyMMdd")},
                    { "limit_day",limit_day.ToString()}
                });

                var Client = HttpClientFactory.CreateClient("KakaoSendAPI");

                List<KaKaoSenderResult> SenderList = new List<KaKaoSenderResult>();

                using (var response = await Client.PostAsync("list/", Content).ConfigureAwait(false))
                {
                    HttpResponseResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    JObject? Jobj = JObject.Parse(HttpResponseResult);
                    JToken? AligoList = Jobj["list"];

                    if (AligoList is null)
                        return new ResponseList<KaKaoSenderResult>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };

                    int count = AligoList.Count();
                    foreach (JToken token in AligoList)
                    {
                        KaKaoSenderResult item = new KaKaoSenderResult();
                        item.Sender = token["sender"]?.ToString() ?? String.Empty;
                        item.Message = token["msg"]?.ToString() ?? String.Empty;
                        item.SendDate = DateTime.Parse(token["reg_date"]!.ToString());
                        if (token["fail_count"]!.ToString() == "0")
                            item.Result = "성공";
                        else
                            item.Result = "실패";

                        SenderList.Add(item);
                    }
                }

                return new ResponseList<KaKaoSenderResult>() { message = "요청이 정상 처리되었습니다.", data = SenderList, code = 200};
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<KaKaoSenderResult>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 민원인 전용 VOC 등록 확인메시지
        /// </summary>
        /// <returns></returns>
        public async Task<AddKakaoLogDTO?> AddVocAnswer(string title, string receiptnum, DateTime receiptdate, string receiver, string url, string placetel)
        {
            try
            {
                // HttpClientFactory에서 HttpClient 가져오기
                var client = HttpClientFactory.CreateClient("KakaoSendAPI");

                // [1] 토큰 생성 요청
                using var tokenContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId }
                });

                using var tokenResponse = await client.PostAsync("akv10/token/create/30/s/", tokenContent).ConfigureAwait(false);
                var tokenResponseResult = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                var tokenObject = JObject.Parse(tokenResponseResult);
                string? token = Convert.ToString(tokenObject["token"]);
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                // [2] 메시지 작성
                string year = receiptdate.ToString("yyyy"); // 년
                string month = receiptdate.ToString("MM");  // 월
                string day = receiptdate.ToString("dd");    // 일

                string message = $"민원이 접수되었습니다.\n■제목: {title}\n■접수번호: {receiptnum}\n■접수일: {year}년{month}월{day}일\n■문의전화: {placetel}";

                var buttonValue = new JObject
                {
                    { "name", "열람하기" },
                    { "linkType", "WL" },
                    { "linkTypeName", "웹 링크" },
                    { "linkMo", url },
                    { "linkPc", url }
                };

                var buttonArray = new JArray { buttonValue };
                var buttonResult = new JObject { { "button", buttonArray } };

                // [3] 알림톡 전송 요청
                using var messageContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId },
                    { "token", token },
                    { "senderkey", Common.KakaoSenderKey },
                    { "tpl_code", Common.KakaoTemplateCode_1 },
                    { "sender", Common.KakaoSenders },
                    { "receiver_1", receiver },
                    { "subject_1", title },
                    { "message_1", message },
                    { "button_1", buttonResult.ToString() },
                    { "failover", "Y" },
                    { "fsubject_1", title },
                    { "fmessage_1", message }
                });

                messageContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                using var messageResponse = await client.PostAsync("akv10/alimtalk/send/", messageContent).ConfigureAwait(false);
                messageResponse.EnsureSuccessStatusCode();
                var messageResponseResult = await messageResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                // [4] 결과 처리
                var messageObject = JObject.Parse(messageResponseResult);

                var logDTO = new AddKakaoLogDTO
                {
                    Code = Convert.ToString(messageObject["code"]),
                    Message = Convert.ToString(messageObject["message"]),
                    MSGID = Convert.ToString(messageObject["info"]?["mid"]?.ToString()), // 메시지 ID
                    Phone = receiver // 받는사람 전화번호
                };

                return logDTO;
            }
            catch (Exception ex)
            {
                // 예외 처리 및 로깅
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }

            #region 이전버전
            /*
        try
        {
            // [1]. 토큰생성
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"apikey", Common.KakaoAPIKey },
                { "userid", Common.KakaoUserId }
            });

            HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/token/create/30/s/", Content).ConfigureAwait(false);
            HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

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
                { "button_1",buttonResult.ToString()},
                { "failover","Y"},
                { "fsubject_1",title},
                { "fmessage_1",message}
            });

            HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/alimtalk/send/", Content).ConfigureAwait(false);
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "euc-kr"
            };


            HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            Jobj = JObject.Parse(HttpResponseResult);

            AddKakaoLogDTO LogDTO = new AddKakaoLogDTO();
            LogDTO.Code = Convert.ToString(Jobj["code"]);
            LogDTO.Message = Convert.ToString(Jobj["message"]);
            LogDTO.MSGID = Convert.ToString(Jobj["info"]?["mid"]?.ToString()); // 메시지 ID
            LogDTO.Phone = receiver; // 받는사람 전화번호

            return LogDTO;
        }
        catch(Exception ex)
        {
            LogService.LogMessage(ex.ToString());
#if DEBUG
            CreateBuilderLogger.ConsoleLog(ex);
#endif
            return null;
        }
            */
            #endregion
        }

        /// <summary>
        /// 민원인 전용 VOC 상태변경 메시지
        /// </summary>
        /// <param name="receiptnum">접수번호</param>
        /// <param name="status">진행상태</param>
        /// <param name="receiver">받는사람 전화번호</param>
        /// <param name="url">링크 URL</param>
        /// <param name="placetel">사업장 전화번호</param>
        /// <returns></returns>
        public async Task<AddKakaoLogDTO?> UpdateVocAnswer(string receiptnum, string status, string receiver, string url, string placetel)
        {

            try
            {
                var client = HttpClientFactory.CreateClient("KakaoSendAPI");

                // [1] 토큰 생성요청
                using var tokenContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId }
                });

                using var tokenResponse = await client.PostAsync("akv10/token/create/30/s/", tokenContent).ConfigureAwait(false);
                var tokenResponseResult = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                JObject Jobj = JObject.Parse(tokenResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                string message = $"■접수번호: {receiptnum}의 진행사항이 변경되었습니다.\n■진행상태: {status}\n■문의전화: {placetel}";

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
                    { "button_1",buttonResult.ToString()},
                    { "failover","Y"},
                    { "fsubject_1","진행사항 변경"},
                    { "fmessage_1",message}
                });

                using var HttpResponse = await client.PostAsync("akv10/alimtalk/send/", Content).ConfigureAwait(false);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                Jobj = JObject.Parse(HttpResponseResult);

                AddKakaoLogDTO LogDTO = new AddKakaoLogDTO();
                LogDTO.Code = Convert.ToString(Jobj["code"]);
                LogDTO.Message = Convert.ToString(Jobj["message"]);
                LogDTO.MSGID = Convert.ToString(Jobj["info"]?["mid"]?.ToString()); // 메시지 ID
                LogDTO.Phone = receiver; // 받는사람 전화번호
                return LogDTO;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }

            #region 이전버전
            /*
            try
            {
                // [1]. 토큰생성
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/token/create/30/s/", Content).ConfigureAwait(false);
                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                JObject? Jobj = JObject.Parse(HttpResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (String.IsNullOrWhiteSpace(token))
                    return null;

                string message = $"■접수번호: {receiptnum}의 진행사항이 변경되었습니다.\n■진행상태: {status}\n■문의전화: {placetel}";

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
                    { "button_1",buttonResult.ToString()},
                    { "failover","Y"},
                    { "fsubject_1","진행사항 변경"},
                    { "fmessage_1",message}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/alimtalk/send/", Content).ConfigureAwait(false);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                Jobj = JObject.Parse(HttpResponseResult);

                AddKakaoLogDTO LogDTO = new AddKakaoLogDTO();
                LogDTO.Code = Convert.ToString(Jobj["code"]);
                LogDTO.Message = Convert.ToString(Jobj["message"]);
                LogDTO.MSGID = Convert.ToString(Jobj["info"]?["mid"]?.ToString()); // 메시지 ID
                LogDTO.Phone = receiver; // 받는사람 전화번호
                return LogDTO;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
    #if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
    #endif
                return null;
            }
            */
            #endregion
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

        /// <summary>
        /// 랜덤 인증코드 생성
        /// </summary>
        /// <returns></returns>
        public string RandomVerifyAuthCode()
        {
            RandomGenerator generator = new RandomGenerator();
            string randomPassword = generator.RandomVerifyCode();
            return randomPassword;
        }

        /// <summary>
        /// 인증코드 발급
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="authcode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> AddVerifyAuthCodeAnser(string buildingname,string phonenumber, string authcode)
        {
            try
            {
                // HttpClientFactory에서 HttpClient 가져오기
                var client = HttpClientFactory.CreateClient("KakaoSendAPI");

                // [1] 토큰 생성 요청
                using var tokenContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey },
                    { "userid", Common.KakaoUserId }
                });

                using var tokenResponse = await client.PostAsync("akv10/token/create/30/s/", tokenContent).ConfigureAwait(false);
                var tokenResponseResult = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                JObject? Jobj = JObject.Parse(tokenResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (String.IsNullOrWhiteSpace(token))
                    return false;

                string message = $"인증번호는 {authcode} 입니다.";

                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId},
                    { "token", token},
                    { "senderkey", Common.KakaoSenderKey},
                    { "tpl_code", Common.KakaoTemplateCode_4},
                    { "sender", Common.KakaoSenders},
                    { "receiver_1",phonenumber},
                    { "subject_1","인증코드 발급"},
                    { "message_1", message},
                    { "failover","Y"},
                    { "fsubject_1","인증코드 발급"},
                    { "fmessage_1",message}
                });

                using var HttpResponse = await client.PostAsync("akv10/alimtalk/send/", Content).ConfigureAwait(false);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                Jobj = JObject.Parse(HttpResponseResult);

                if (Jobj is not null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }

            #region 이전버전
            /*
            try
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/token/create/30/s", Content).ConfigureAwait(false);
                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                JObject? Jobj = JObject.Parse(HttpResponseResult);
                string? token = Convert.ToString(Jobj["token"]);
                if (String.IsNullOrWhiteSpace(token))
                    return false;

                string message = $"인증번호는 {authcode} 입니다.";

                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "apikey", Common.KakaoAPIKey},
                    { "userid", Common.KakaoUserId},
                    { "token", token},
                    { "senderkey", Common.KakaoSenderKey},
                    { "tpl_code", Common.KakaoTemplateCode_4},
                    { "sender", Common.KakaoSenders},
                    { "receiver_1",phonenumber},
                    { "subject_1","인증코드 발급"},
                    { "message_1", message},
                    { "failover","Y"},
                    { "fsubject_1","인증코드 발급"},
                    { "fmessage_1",message}
                });

                HttpResponse = await Common.HttpClient.PostAsync("https://kakaoapi.aligo.in/akv10/alimtalk/send/", Content).ConfigureAwait(false);
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "euc-kr"
                };

                HttpResponseResult = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                Jobj = JObject.Parse(HttpResponseResult);

                //AddKakaoLogDTO LogDTO = new AddKakaoLogDTO();
                //LogDTO.Code = Convert.ToString(Jobj["code"]);
                //LogDTO.Message = Convert.ToString(Jobj["message"]);
                //LogDTO.MSGID = Convert.ToString(Jobj["info"]?["mid"]?.ToString()); // 메시지ID
                //LogDTO.Phone = phonenumber;

                //return LogDTO;

                if (Jobj is not null)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            */
            #endregion
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

            /// <summary>
            /// 랜덤코드 생성
            /// </summary>
            /// <returns></returns>
            public string RandomVerifyCode()
            {
                var VertifyBuilder = new StringBuilder();

                // 1000 에서 9999 사이의 4자리 숫자
                int number = RandomNumber(0, 9999);       // 0부터 9999까지 숫자 생성
                VertifyBuilder.Append(number.ToString("D4"));

                return VertifyBuilder.ToString();
            }
                        

            public string RandomPassword()
            {
                var passwordBuilder = new StringBuilder();

                // ** Version 1
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
