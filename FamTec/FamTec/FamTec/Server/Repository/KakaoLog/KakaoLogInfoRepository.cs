using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.KakaoLog
{
    public class KakaoLogInfoRepository : IKakaoLogInfoRepository
    {
        private readonly WorksContext context;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<KakaoLogInfoRepository> CreateBuilderLogger;
        
        //private readonly HttpClient HttpClient;
        private HttpResponseMessage? HttpResponse;


        public KakaoLogInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<KakaoLogInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 카카오 알림톡 발송로그 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<KakaoLogTb?> AddAsync(KakaoLogTb model)
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
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 카카오 알림톡 발송로그 전체 조회
        /// </summary>
        /// <returns></returns>
        public async Task<List<KakaoLogTb>?> GetKakaoLogList(int placeid, int isSuccess)
        {
            try
            {
                List<KakaoLogTb>? model = null;

                if(isSuccess == 0) // 전체
                {
                    model = await context.KakaoLogTbs
                    .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.MsgUpdate == true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);
                }
                else if(isSuccess == 1) // 성공
                {
                    model = await context.KakaoLogTbs
                  .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.MsgUpdate == true && m.Rslt == "0")
                  .OrderBy(m => m.CreateDt)
                  .ToListAsync()
                  .ConfigureAwait(false);
                }
                else // 실패
                {
                    model = await context.KakaoLogTbs
                .Where(m => m.DelYn != true && m.PlaceTbId == placeid && m.MsgUpdate == true && m.Rslt != "0")
                .OrderBy(m => m.CreateDt)
                .ToListAsync()
                .ConfigureAwait(false);
                }
           

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }


        /// <summary>
        /// 카카오 알림톡 발송로그 기간별 전체 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public async Task<List<KakaoLogTb>?> GetKakaoDateLogList(int placeid, DateTime startdate, DateTime enddate, int isSuccess)
        {
            try
            {
                // isSuccess : 0 전체 1: 성공 2: 실패
                List<KakaoLogTb>? model = null;

                if (isSuccess == 0) // 전체
                {
                    model = await context.KakaoLogTbs
                       .Where(m => m.DelYn != true &&
                       m.PlaceTbId == placeid &&
                       m.MsgUpdate == true &&
                       m.CreateDt >= startdate && m.CreateDt <= enddate.AddDays(1))
                       .ToListAsync()
                       .ConfigureAwait(false);
                }
                else if(isSuccess == 1) // 성공
                {
                    model = await context.KakaoLogTbs
                 .Where(m => m.DelYn != true &&
                 m.PlaceTbId == placeid &&
                 m.MsgUpdate == true &&
                 m.Rslt == "0" &&
                 m.CreateDt >= startdate && m.CreateDt <= enddate.AddDays(1))
                 .ToListAsync()
                 .ConfigureAwait(false);
                }
                else
                {
                    model = await context.KakaoLogTbs
                 .Where(m => m.DelYn != true &&
                 m.PlaceTbId == placeid &&
                 m.MsgUpdate == true &&
                 m.Rslt != "0" &&
                 m.CreateDt >= startdate && m.CreateDt <= enddate.AddDays(1))
                 .ToListAsync()
                 .ConfigureAwait(false);
                }

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생 : {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
        /// 카카오 알림톡 발송록 기간별 조회
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async Task<List<KakaoLogTb>?> GetKakaoLogList(DateTime StartDate, DateTime EndDate)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 사업장에 속해있는 카카오 알림톡 발송로그 카운트 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int?> GetKakaoLogCount(int placeid)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
        public async Task<List<KakaoLogTb>?> GetKakaoLogPageNationList(int placeid, int pagenumber, int pagesize)
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
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

   
    }
}