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
                context.KakaoLogTbs.Add(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
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
                    //.Take(1000)
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid)
                    .ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
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
                    .OrderBy(m => m.CreateDt).ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
