using FamTec.Server.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FamTec.Server
{
    public class AuthCodeService
    {
        private readonly IMemoryCache MemoryCache;
        private readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(3);

        private readonly ILogService LogService;
        private readonly ConsoleLogService<AuthCodeService> CreateBuilderLogger;

        public AuthCodeService(IMemoryCache _memorycache,
            ILogService _logservice,
            ConsoleLogService<AuthCodeService> _createbuilderlogger)
        {
            this.MemoryCache = _memorycache;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        // 인증번호를 저장하거나 갱신하는 메서드
        public async Task<bool> SaveOrUpdateAuthCode(string userid, string authcode)
        {
            try
            {
                // 동일한 ID로 기존 인증번호가 있으면 삭제
                this.MemoryCache.Remove(userid);

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ExpirationTime
                };

                // 해당 ID로 인증번호 발급
                //this.MemoryCache.Set(userid, authcode, cacheOptions);
                // 해당 ID로 인증번호 발급
                await Task.Run(() => this.MemoryCache.Set(userid, authcode, cacheOptions));

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return await Task.FromResult(false);
            }
        }

        // 인증번호 확인 및 데이터 삭제 메서드
        public async Task<bool> CheckVerifyAuthCode(string userid, string authcode)
        {
            try
            {
                // 메모리 캐시에서 인증번호를 조회
                if (this.MemoryCache.TryGetValue(userid, out string cacheCode))
                {
                    if (cacheCode == authcode)
                    {
                        this.MemoryCache.Remove(userid); // 데이터 삭제
                        return await Task.FromResult(true);
                    }
                }
                return await Task.FromResult(false);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return await Task.FromResult(false);
            }
        }

        /*
        public async Task<int> MemoryChacheCount()
        {
            if (this.MemoryCache is MemoryCache memoryCache)
            {
                return memoryCache.Count;
            }
            throw new InvalidOperationException("Unable to determine cache count.");
        }
        */
    }
}
