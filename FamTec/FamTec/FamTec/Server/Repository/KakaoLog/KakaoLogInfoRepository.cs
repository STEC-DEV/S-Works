using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.KakaoLog
{
    public class KakaoLogInfoRepository : IKakaoLogInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public KakaoLogInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 카카오 알림톡 발송로그 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<KakaoLogTb?> AddAsync(KakaoLogTb model)
        {
            try
            {
                await context.KakaoLogTbs.AddAsync(model).ConfigureAwait(false);
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 카카오 알림톡 발송로그 전체 조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<KakaoLogTb>?> GetKakaoLogList(int placeid)
        {
            try
            {
                List<KakaoLogTb>? model = await context.KakaoLogTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 카카오 알림톡 발송록 기간별 조회
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async ValueTask<List<KakaoLogTb>?> GetKakaoLogList(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                List<KakaoLogTb>? model = await context.KakaoLogTbs
                    .Where(m => m.DelYn != true && m.CreateDt >= StartDate && m.CreateDt <= EndDate)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 사업장에 속해있는 카카오 알림톡 발송로그 카운트 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<int?> GetKakaoLogCount(int placeid)
        {
            try
            {
                int count = await context.KakaoLogTbs
                    .Where(m => m.PlaceTbId == placeid && 
                                m.DelYn != true)
                    .CountAsync()
                    .ConfigureAwait(false);

                return count;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 카카오 알림톡 발송로그 리스트 페이지네이션 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async ValueTask<List<KakaoLogTb>?> GetKakaoLogPageNationList(int placeid, int pagenumber, int pagesize)
        {
            try
            {
                List<KakaoLogTb>? model = await context.KakaoLogTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        
    }
}
