using Azure.Core;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using Microsoft.IdentityModel.Tokens;
using System.Net;

//using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamTec.Client.Middleware
{
    public class ApiManager
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public ApiManager(CustomAuthenticationStateProvider authStateProvider)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://123.2.156.148:5245/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _authStateProvider = authStateProvider;
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _authStateProvider.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /*
         get 요청 메서드
         */
        private async Task<T> GetSendRequestAsync<T>(HttpMethod method, string endpoint, Dictionary<string, int> queryParams = null, object data = null)
        {
            await AddAuthorizationHeader();

            var request = new HttpRequestMessage(method, endpoint);
            
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        /*
         * Post 요청
         */
        //private async Task<string> PostSendReqeustAsync(string endpoint, object data, string contentsType)
        //{
        //    await AddAuthorizationHeader();

        //    var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

        //    if (data != null)
        //    {
        //        string json = JsonSerializer.Serialize(data);
        //        request.Content = new StringContent(json, Encoding.UTF8, contentsType);
        //    }

        //    HttpResponseMessage response = await _httpClient.SendAsync(request);
        //    var a= response.Content;
        //    Console.WriteLine(a);
        //    response.EnsureSuccessStatusCode();

        //    return await response.Content.ReadAsStringAsync();

        //}

        private async Task<string> PostSendReqeustAsync(string endpoint, object data, string contentType)
        {
            await AddAuthorizationHeader();

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (data is HttpContent httpContent)
            {
                request.Content = httpContent;
            }
            else if (data != null)
            {
                if (contentType == "application/json")
                {
                    string json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, contentType);
                }
                else if (contentType == "multipart/form-data")
                {
                    await Console.Out.WriteLineAsync(contentType);
                    var multipartContent = new MultipartFormDataContent();

                    foreach (var prop in data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(data);
                        if (value != null)
                        {
                            if (value is byte[] fileBytes)
                            {
                                multipartContent.Add(new ByteArrayContent(fileBytes), prop.Name, prop.Name);
                            }
                            else
                            {
                                multipartContent.Add(new StringContent(value.ToString()), prop.Name, contentType);
                            }
                        }
                    }

                    request.Content = multipartContent;
                }
                else
                {
                    throw new ArgumentException($"Unsupported content type: {contentType}");
                }
            }

            if (request.Content != null && request.Content.Headers.ContentType == null)
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
            Console.WriteLine($"Final Content-Type: {request.Content.Headers.ContentType}"); // 최종 Content-Type 로깅
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync();
        }

        /*
         * Put 요청
         */
        private async Task<string> PutSendRequestAsync(string endpoint, object data)
        {
            await AddAuthorizationHeader();

            var request = new HttpRequestMessage(HttpMethod.Put, endpoint);

            if (data != null)
            {
                string json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }



        //로그인
        public async Task<ResponseUnit<T>> PostLoginAsync<T>(string endpoint, LoginDTO user)
        {
            string contentType = "application/json";
            string jsonResponse = await PostSendReqeustAsync(endpoint, user, contentType);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
        }

        //post 공통
        public async Task<ResponseUnit<T>> PostAsync<T>(string endpoint, object user)
        {
            string contentsType = "application/json";
            string jsonResponse = await PostSendReqeustAsync(endpoint, user, contentsType);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
        }

        //Post Image
        public async Task<ResponseUnit<T>> PostWithFilesAsync<T>(string endPoint, object manager)
        {
            string contentType = "multipart/form-data";
            using var content = new MultipartFormDataContent();
            // DTO 속성들을 폼 데이터로 추가
            var properties = manager.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(manager);
                if (value != null)
                {
                    switch (prop.Name)
                    {
                        case "Image":
                            if (value is byte[] imageBytes && imageBytes.Length > 0)
                            {
                                //var imageContent = new ByteArrayContent(imageBytes);
                                //content.Add(imageContent, "Image", manager.GetType().GetProperty("ImageName")?.GetValue(manager)?.ToString() ?? "image.jpg");

                                var stream = new MemoryStream(imageBytes);
                                var streamContent = new StreamContent(stream);
                                content.Add(content : streamContent, name : "\"files\"", fileName : manager.GetType().GetProperty("ImageName")?.GetValue(manager)?.ToString() ?? "image.jpg");
                            }
                            break;
                        case "PlaceList":
                            if (value is List<int> placeList)
                            {
                                foreach (var place in placeList)
                                {
                                    content.Add(new StringContent(place.ToString()), "PlaceList");
                                }
                            }
                            break;
                        default:
                            content.Add(new StringContent(value.ToString()), prop.Name);
                            break;
                    }
                }
            }
            string jsonResponse = await PostSendReqeustAsync(endPoint, content, contentType);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
        }
        

        //put 공통
        public async Task<ResponseUnit<T>> PutAsync<T>(string endpoint, object data)
        {
            string jsonResponse = await PutSendRequestAsync(endpoint, data);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
        }


        //ResponseUinit Get
        public async Task<ResponseUnit<T>> GetUnitAsync<T>(string endpoint)
        {
            //var queryParams = new Dictionary<string, int> { { "placeid", placeId } };
            return await GetSendRequestAsync<ResponseUnit<T>>(HttpMethod.Get, endpoint);
        }


        //ResponseList Get
        public async Task<ResponseList<T>> GetListAsync<T>(string endpoint)
        {
            return await GetSendRequestAsync<ResponseList<T>> (HttpMethod.Get, endpoint);
        }

        // 사업장 삭제 (PUT 요청)
        public async Task<ResponseUnit<T>> DeletePlaceAsync<T>(string endpoint, object data)
        {
            string jsonResponse =  await PutSendRequestAsync(endpoint, data);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
                
        }



    }

}
