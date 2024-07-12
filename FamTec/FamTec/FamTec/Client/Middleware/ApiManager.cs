using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private async Task<string> PostSendReqeustAsync(string endpoint, object data)
        {
            await AddAuthorizationHeader();

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (data != null)
            {
                string json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            var a= response.Content;
            Console.WriteLine(a);
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
            string jsonResponse = await PostSendReqeustAsync(endpoint, user);
            return JsonSerializer.Deserialize<ResponseUnit<T>>(jsonResponse);
        }

        //post 공통
        public async Task<ResponseUnit<T>> PostAsync<T>(string endpoint, object user)
        {
            string jsonResponse = await PostSendReqeustAsync(endpoint, user);
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
