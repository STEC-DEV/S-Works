
using DevExpress.Office.Utils;
using StackExchange.Redis;

namespace FamTec.Server.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase Redis;
        private readonly ILogService LogService;
        private static readonly TimeSpan TTL = TimeSpan.FromMinutes(3); // 유효시간
        private readonly ConsoleLogService<RedisService> CreateBuilderLogger;
        private static readonly Random Randoms = new Random();

        public RedisService(IConnectionMultiplexer _muxer,
            ILogService _logservice,
            ConsoleLogService<RedisService> _createbuilderlogger)
        {
            this.Redis = _muxer.GetDatabase();
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

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

     
    }
}
