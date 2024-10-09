using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace FamTec.Server.Middleware
{
    public class DuplicateRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        private const int RequestLimit = 300; // 5초 내에 허용할 최대 요청 횟수
        private const int CacheExpirationInSeconds = 5; // 캐시 유지 시간 (초)

        public DuplicateRequestMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 제외할 API 경로 목록
            var excludedPaths = new List<string>
            {
                "/api/Alarm/sign/GetAlarmList",
                "/api/Alarm/sign/GetAlarmDateList",
                "/api/Alarm/sign/AllAlarmDelete",
                "/api/Alarm/sign/AlarmDelete",
                "/api/AdminUser/sign/UserIdCheck"
            };

            // 현재 요청 경로 확인
            var currentPath = context.Request.Path.ToString();

            // 만약 현재 경로가 제외 목록에 있다면, 다음 미들웨어로 전달하고 종료
            if (excludedPaths.Contains(currentPath))
            {
                await _next(context);
                return;
            }


            // 요청 키 생성 (요청 URL + 쿼리스트링 + HTTP 메서드를 조합하여 고유 키를 생성)
            var requestKey = GenerateRequestKey(context);

            
            // 캐시에서 현재 요청 카운트를 확인하거나 없으면 0으로 설정
            if (_cache.TryGetValue(requestKey, out int requestCount))
            {
                Console.WriteLine(requestCount);
                if (requestCount >= RequestLimit)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // 429 상태 코드 (요청이 너무 많음)
                    await context.Response.WriteAsync($"10초 내에 동일한 요청이 {RequestLimit}번 초과되어 차단되었습니다.");
                    return;
                }

                // 요청 횟수 증가
                _cache.Set(requestKey, ++requestCount, TimeSpan.FromSeconds(CacheExpirationInSeconds));
            }
            else
            {
                Console.WriteLine(requestCount);
                // 첫 요청일 경우 캐시에 1로 저장
                _cache.Set(requestKey, 1, TimeSpan.FromSeconds(CacheExpirationInSeconds));
            }

            await _next(context); // 다음 미들웨어 또는 컨트롤러로 요청 전달
        }

        // 요청 키 생성
        private string GenerateRequestKey(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var query = context.Request.QueryString.ToString();
            var method = context.Request.Method;

            // 클라이언트 식별자 추가 (IP 주소 또는 Authorization 헤더 등)
            var clientIdentifier = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // 만약 JWT 토큰으로 클라이언트를 식별한다면:
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                clientIdentifier = authorizationHeader.ToString();
            }

            var rawKey = $"{clientIdentifier}:{method}:{path}:{query}";
            Console.WriteLine(rawKey);

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawKey);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
