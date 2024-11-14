using Blazored.LocalStorage;
using Irony.Parsing;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace FamTec.Client.Shared.Provider
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {

        private readonly ILocalStorageService _localStorageService;

        public CustomAuthProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        //토큰 유저 리턴
        public async Task<ClaimsPrincipal> ReturnUserClaim()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            return user;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            
            var jwtToken = await _localStorageService.GetItemAsync<string>("sworks-jwt-token");
            //Console.WriteLine($"JWT Token: {(string.IsNullOrEmpty(jwtToken) ? "Not found" : "Found")}");

            if (string.IsNullOrEmpty(jwtToken))
            {
                //Console.WriteLine("No JWT token found, returning unauthenticated state");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(jwtToken);
            var identity = new ClaimsIdentity(claims, "JwtAuth");
            var user = new ClaimsPrincipal(identity);
            //Console.WriteLine($"Authentication state created. IsAuthenticated: {identity.IsAuthenticated}");

            return new AuthenticationState(user);
        }


        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                //Console.WriteLine($"Parsed {keyValuePairs.Count} claims from JWT");

                return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JWT: {ex.Message}");
                return Enumerable.Empty<Claim>();
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        public async Task<string> GetUserName()
        {
            var user = await ReturnUserClaim();
            string name = user.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
            return name;
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
            if (placeIdxClaim != null)
            {
                return int.Parse(placeIdxClaim.Value);
            }

            return -1; // PlaceIdx 클레임이 없는 경우
        }


        //token UserPerms 내부 권한 확인
        public async Task<int> GetUserPermission(string permName)
        {
            //Console.WriteLine(permName);
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userPerms = user.Claims.FirstOrDefault(c => c.Type == "UserPerms")?.Value;
            if (string.IsNullOrEmpty(userPerms)) return 0;

            try
            {
                var perms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPerms);

                if (perms != null && perms.TryGetValue(permName, out string permValueString))
                {
                    if (int.TryParse(permValueString, out int permValue))
                    {
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

        //token VocPemrs 내부 권한 단일 확인 
        public async Task<bool> GetUserVocPermission(string permName)
        {
            //Console.WriteLine(permName);
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userPerms = user.Claims.FirstOrDefault(c => c.Type == "VocPerms")?.Value;
            if (string.IsNullOrEmpty(userPerms)) return false;

            try
            {
                var perms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPerms);

                if (perms != null && perms.TryGetValue(permName, out string permValueString))
                {
                    if (bool.TryParse(permValueString, out bool permValue))
                    {
                        return permValue;
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing UserVocPerms: {ex.Message}");
            }

            return false; // 권한이 없거나 파싱할 수 없는 경우 0 반환
        }

        //token UserPerms 내부 권한 확인
        public async Task<bool> GetPlacePermission(string permName)
        {
            //Console.WriteLine(permName);
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userPerms = user.Claims.FirstOrDefault(c => c.Type == "PlacePerms")?.Value;
            if (string.IsNullOrEmpty(userPerms)) return false;

            try
            {
                var perms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPerms);

                if (perms != null && perms.TryGetValue(permName, out string permValueString))
                {
                    if (bool.TryParse(permValueString, out bool permValue))
                    {
                        return permValue;
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing UserPerms: {ex.Message}");
            }

            return false; // 권한이 없거나 파싱할 수 없는 경우 0 반환
        }

        //사업장 이름 조회
        public async Task<string> GetPlaceName()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            Console.WriteLine("==============================");
            foreach (var c in user.Claims)
            {
                
                Console.WriteLine(c.Type);
                Console.WriteLine(c.Value);
                
            }
            Console.WriteLine("==============================");
            var claim = user.FindFirst(c => c.Type == "PlaceName");
            
            Console.WriteLine("클레임"+claim);
            if (claim == null) return null;
            
            string name = claim.Value;
            if (string.IsNullOrEmpty(name)) return null;
            
            return name;
        }

        public async Task<DateTime> GetPlaceCreate()
        {
            var user = await ReturnUserClaim();
            var createDate = user.Claims.FirstOrDefault(c => c.Type == "PlaceCreateDT").Value;
            return Convert.ToDateTime(createDate);
        }

        //알람 권한 조회
        public async Task<bool> GetAlarmYN()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            // AlarmYN 클레임을 찾아서 값 추출
            var alarmClaim = user.FindFirst(c => c.Type == "AlarmYN")?.Value;

            // 값이 null이 아니면 bool로 변환하고, 그렇지 않으면 기본값 false 반환
            if (!string.IsNullOrEmpty(alarmClaim) && bool.TryParse(alarmClaim, out bool alarm))
            {
                return alarm;
            }

            return false;
        }
        
        //userPerm 전체 List<bool>반환
        public async Task<List<bool>> GetAllUserPerm()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userPerm = user.Claims.FirstOrDefault(c => c.Type == "VocPerms")?.Value;
            if(String.IsNullOrEmpty(userPerm))
            {
                return null;
            }
            try
            {
                var perms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPerm);
                if (perms != null)
                {
                    var permValues = new List<bool>();
                    foreach (var value in perms.Values)
                    {
                        if (bool.TryParse(value, out bool boolValue))
                        {
                            permValues.Add(boolValue);
                        }
                        else if (int.TryParse(value, out int intValue))
                        {
                            permValues.Add(intValue != 0);
                        }
                        else
                        {
                            // 변환할 수 없는 값은 false로 처리
                            permValues.Add(false);
                        }
                    }
                    return permValues;
                }

            }
            catch(JsonException ex)
            {
                Console.WriteLine("all userPerm Return Error" + ex);
            }


            return null;
        }


        /// <summary>
        /// 설비 사용자 권한
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetFacilityUserPemrList()
        {
            var user = await ReturnUserClaim();

            // UserPerms를 파싱합니다.
            var userPermsJson = user.Claims.FirstOrDefault(c => c.Type == "UserPerms")?.Value;
            if (string.IsNullOrEmpty(userPermsJson))
            {
                throw new Exception("UserPerms 정보가 없습니다.");
            }
            // JSON 파싱
            var userPerms = JsonSerializer.Deserialize<Dictionary<string, string>>(userPermsJson);
            var permList = userPerms.Values.Select(v => int.Parse(v)).ToList();
            var facList = new List<int>(permList.GetRange(1,8));
            return facList;
        }


        //사용자 id 조회 in 토큰
        public async Task<int> GetUserId()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == "UserIdx")?.Value;
            if(String.IsNullOrEmpty(userId))
            {
                return 0;
            }
            else
            {
                return int.Parse(userId);
            }
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

        public async Task NotifyAuthState()
        {
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task NotifyLogout()
        {
            // 비로그인 상태로 설정
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }

        //설비 읽기권한 단일 조회
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


        //===============관리자=====================
        public async Task<int> GetAdminId()
        {
            var user = await ReturnUserClaim();
            var adminidx = user.Claims.FirstOrDefault(c => c.Type == "AdminIdx").Value;
            if(adminidx == null) { return 0; }
            return int.Parse(adminidx);
        }
    }
}
