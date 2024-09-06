using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace FamTec.Client.Shared.Provider
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {

        private readonly ILocalStorageService _localStorageService;

        public CustomAuthProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var jwtToken = await _localStorageService.GetItemAsync<string>("sworks-jwt-token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()));
            }
            

            return new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(ParseClaimsFromJwt(jwtToken),"JwtAuth")));
        }


        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        //관리자에서 [시스템 관리자, 마스터] / [매니저] 구분 write권한
        public async Task<string> AdminRole()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            if(roleClaim != null)
            {
                return roleClaim.Value;
            }
            return null;
        }

        //일반 사용자 관리자 구분
        public async Task<bool> IsAdminAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            return user.Claims.Any(c => c.Type == "AdminYN" && c.Value == "True");
        }

        public async Task<int> GetUserPermission(string permName)
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userPerms = user.Claims.FirstOrDefault(c => c.Type == "UserPerms")?.Value;
            if (string.IsNullOrEmpty(userPerms)) return 0;

            try
            {
                var perms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPerms);
                if (perms != null && perms.ContainsKey(permName))
                {
                    if (int.TryParse(perms[permName], out int permValue))
                    {
                        Console.WriteLine(permName);
                        Console.WriteLine(permValue);
                        return permValue;
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing UserPerms: {ex.Message}");
            }

            return 0; // 권한이 없거나 파싱할 수 없는 경우 0 반환
        }
        

        public async Task<bool> GetLoginMode()
        {
            bool loginMode = await _localStorageService.GetItemAsync<bool>("loginMode");
            return loginMode;
        }

        public async Task<string> GetJwtTokenAsync()
        {
            return await _localStorageService.GetItemAsync<string>("sworks-jwt-token");
        }

        public void NotifyAuthState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyLogout()
        {
            // 비로그인 상태로 설정
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
