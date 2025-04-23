using StackExchange.Redis;
using System.Security.Cryptography;

namespace FamTec.Server.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer Muxer;
        private readonly IDatabase Redis;
        private readonly ILogService LogService;
        private static readonly TimeSpan TTL = TimeSpan.FromMinutes(3); // 유효시간
        private readonly ConsoleLogService<RedisService> CreateBuilderLogger;
        private static readonly Random Randoms = new Random();

        public RedisService(IConnectionMultiplexer _muxer,
            ILogService _logservice,
            ConsoleLogService<RedisService> _createbuilderlogger)
        {
            Muxer = _muxer;
            this.Redis = Muxer.GetDatabase();
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        #region 카카오톡 인증코드 관련
        /// <summary>
        /// Redis 코드 저장
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<string> SetCodeAsync(string phoneNumber)
        {
            try
            {
                var redisKey = $"Verify:KakaoCode:{phoneNumber}";

                // 1) 이전 코드 완전 삭제
                await Redis.KeyDeleteAsync(redisKey);

                // 4자리 숫자코드
                var code = Randoms.Next(1000, 9999).ToString();

                // 2) 새 코드 생성 및 저장 (TTL 초기화)
                await Redis.StringSetAsync(redisKey, code, expiry: TTL, when: When.Always);
                return code;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetValidateCodeAsync(string phoneNumber, string code)
        {
            try
            {
                var redisKey = $"Verify:KakaoCode:{phoneNumber}";
                var stored = await Redis.StringGetAsync(redisKey);
                if (stored.HasValue && stored == code)
                {
                    // 한 번만 사용하도록 키 삭제
                    await Redis.KeyDeleteAsync(redisKey);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 엑세스 토큰 저장 [유저페이지]
        /// </summary>
        /// <returns>
        /// AccessToken & RefreshToken 반환
        /// </returns>
        public async Task<(string, string, string)?> SetWebUserpageAccessAsync(int pId, string accesstoken, string? sessionId = null)
        {
            try
            {
#if DEBUG
                var ping = await Redis.PingAsync();
                Console.WriteLine($"[DEBUG] Redis Ping: {ping.TotalMilliseconds}ms");


                // 사용 중인 DB 인덱스
                Console.WriteLine($"[DBG] Redis DB Index: {Redis.Database}");

                // 연결된 엔드포인트들
                foreach (var ep in Muxer.GetEndPoints())
                    Console.WriteLine($"[DBG] Redis Endpoint: {ep}");
#endif
                // 랜덤 리프레시 토큰 생성
                var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                // 관리자 로그인이 아닐때
                if(String.IsNullOrWhiteSpace(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();// 동시로그인 허용하기 위해
                    var key = $"Token:userpage:{pId}:{sessionId}";
                    bool ok = await Redis.StringSetAsync(key, refreshToken, expiry: TimeSpan.FromDays(1));
#if DEBUG
                    Console.WriteLine($"[DEBUG] Redis SET {key}: {(ok ? "Success" : "Fail")}");
#endif
                    return (accesstoken, refreshToken, sessionId);
                }
                else
                {
                    // 관리자 로그인일때 이미 세션아이디가 받아서 넘어옴.
                    var key = $"Token:userpage:{pId}:{sessionId}";
                    bool ok = await Redis.StringSetAsync(key, refreshToken, expiry: TimeSpan.FromDays(1));
#if DEBUG
                    Console.WriteLine($"[DEBUG] Redis SET {key}: {(ok ? "Success" : "Fail")}");
#endif
                    return (accesstoken, refreshToken, sessionId);
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        /// <summary>
        /// 리프레쉬 토큰 재발급 [유저페이지]
        /// </summary>
        /// <returns></returns>
        public async Task<string?> WebRotateUserpageRefreshTokenAsync(int pId, string refreshtoken, string sessionId)
        {
            try
            {
                var key = $"Token:userpage:{pId}:{sessionId}";

                // 클라이언트가 보낸 토큰이 저장된 토큰과 일치하는지 검증
                var stored = await Redis.StringGetAsync(key);
                if (stored.IsNullOrEmpty || stored != refreshtoken)
                    return null; // 유효하지 않은 리프레시 토큰

                // 남은 TTL 확인
                var ttl = await Redis.KeyTimeToLiveAsync(key);

                if (!ttl.HasValue)
                    return null;

                string returnRefreshToken = refreshtoken;

                // 만료 임박 (1시간 이하) 리프래쉬 토큰 재발급
                if(ttl.Value < TimeSpan.FromHours(1))
                {
                    var rotated = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                    await Redis.StringSetAsync(key, rotated, expiry: TimeSpan.FromDays(1));
                    returnRefreshToken = rotated;
                }
                else
                {
                    // 그렇지 않으면 TTL만 갱신
                    await Redis.KeyExpireAsync(key, TimeSpan.FromDays(1));
                }

                // 최종 사용할 토큰 리턴
                return returnRefreshToken;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        /// <summary>
        /// Redis 키 삭제
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRefreshTokenAsync(int pid, string sessionId)
        {
            try
            {
                RedisKey[] keys = new[]
                {
                    (RedisKey)$"Token:userpage:{pid}:{sessionId}",
                    (RedisKey)$"Token:settingpage:{pid}:{sessionId}"
                };

                // 여러 키를 한 번에 삭제, 리턴값은 삭제된 키의 개수
                long deletedCount = await Redis.KeyDeleteAsync(keys);
                return deletedCount > 0;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 액세스 토큰 저장 [세팅페이지]
        /// </summary>
        /// <returns></returns>
        public async Task<(string, string, string)?> SetWebSettingpageAccessAsync(int pId, string accesstoken)
        {
            try
            {
#if DEBUG
                var ping = await Redis.PingAsync();
                Console.WriteLine($"[DEBUG] Redis Ping: {ping.TotalMilliseconds}ms");


                // 사용 중인 DB 인덱스
                Console.WriteLine($"[DBG] Redis DB Index: {Redis.Database}");

                // 연결된 엔드포인트들
                foreach (var ep in Muxer.GetEndPoints())
                    Console.WriteLine($"[DBG] Redis Endpoint: {ep}");

#endif
                var sessionId = Guid.NewGuid().ToString(); // 동시로그인 허용하기 위해
                // 랜덤 리프레시 토큰 생성
                var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                // Redis에 저장
                var key = $"Token:settingpage:{pId}:{sessionId}";
                bool ok = await Redis.StringSetAsync(key, refreshToken, expiry: TimeSpan.FromDays(1));
#if DEBUG
                Console.WriteLine($"[DEBUG] Redis SET {key}: {(ok ? "Success" : "Fail")}");
#endif
                return (accesstoken, refreshToken, sessionId);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        /// <summary>
        /// 리프레쉬 토큰 재발급 [세팅페이지]
        /// </summary>
        /// <returns></returns>
        public async Task<string?> WebRotateSettingpageRefreshTokenAsync(int pId, string refreshtoken, string sessionId)
        {
            try
            {
                var key = $"Token:settingpage:{pId}:{sessionId}";

                // 클라이언트가 보낸 토큰이 저장된 토큰과 일치하는지 검증
                var stored = await Redis.StringGetAsync(key);
                if (stored.IsNullOrEmpty || stored != refreshtoken)
                    return null; // 유효하지 않은 리프레시 토큰

                // 남은 TTL 확인
                var ttl = await Redis.KeyTimeToLiveAsync(key);

                if (!ttl.HasValue)
                    return null;

                string returnRefreshToken = refreshtoken;

                // 만료 임박 (1시간 이하) 리프래쉬 토큰 재발급
                if(ttl.Value < TimeSpan.FromHours(1))
                {
                    var rotated = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                    await Redis.StringSetAsync(key, rotated, expiry: TimeSpan.FromDays(1));
                    returnRefreshToken = rotated;
                }
                else
                {
                    // 그렇지 않으면 TTL만 갱신
                    await Redis.KeyExpireAsync(key, TimeSpan.FromDays(1));
                }

                // 최종 사용할 토큰 리턴
                return returnRefreshToken;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }
    }
}
