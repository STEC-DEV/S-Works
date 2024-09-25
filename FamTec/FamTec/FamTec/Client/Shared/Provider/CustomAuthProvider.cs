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

        //관리자 여부조회
        public async Task<bool> IsAdmin()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var AdminYnClaim = user.FindFirst("AdminYN");

            if (AdminYnClaim != null && bool.TryParse(AdminYnClaim.Value, out bool isAdmin))
            {
                return isAdmin;
            }

            return false; // 클레임이 없거나 값이 변환되지 않으면 false 반환
        }

        //일반 사용자 관리자 구분
        public async Task<bool> IsAdminAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            return user.Claims.Any(c => c.Type == "AdminYN" && c.Value == "True");
        }

        //사업장조회
        public async Task<int> GetPlaceIdx()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var placeIdxClaim = user.FindFirst("PlaceIdx");
            Console.WriteLine("사업장" + placeIdxClaim);
            if (placeIdxClaim != null)
            {
                return int.Parse(placeIdxClaim.Value);
            }

            return -1; // PlaceIdx 클레임이 없는 경우
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

        //사업장 이름 조회
        public async Task<string> GetPlaceName()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            string name = user.FindFirst(c => c.Type == "PlaceName").Value;
            return name;
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

        public async Task<bool> HasJwtTokenAsync()
        {
            var jwt = await _localStorageService.GetItemAsync<string>("sworks-jwt-token");
            return !String.IsNullOrEmpty(jwt);
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


        public async Task<bool> HasFacilityReadPerm(string permKey)
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;

            var userPerms = user.Claims.FirstOrDefault(c => c.Type == "UserPerms")?.Value;
            if (string.IsNullOrEmpty(userPerms)) return false;

            try
            {
                //// UserPerms 값을 JSON으로 변환합니다.
                var perms = JsonSerializer.Deserialize<Dictionary<string, int>>(userPerms);

                //// 전달된 키(permKey)가 있는지 확인하고, 해당 값이 0이 아니면 권한이 있는 것으로 간주합니다.
                if (perms != null && perms.ContainsKey(permKey))
                {
                    Console.WriteLine("읽기 권한 : " + perms[permKey]);
                    return perms[permKey] != 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing UserPerms: {ex.Message}");
            }
             // 해당 키가 없거나 값이 0이면 권한 없음
            return false;
        }
    }
}
