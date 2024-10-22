using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.Voc
{
    public class VocCommentRepository : IVocCommentRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<VocCommentRepository> CreateBuilderLogger;

        public VocCommentRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<VocCommentRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 민원에 댓글 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<CommentTb?> AddAsync(CommentTb model)
        {
            try
            {
                await context.CommentTbs.AddAsync(model).ConfigureAwait(false);
             
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if(AddResult)
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

        public async Task<List<CommentTb>?> GetCommentList(int vocid)
        {
            try
            {
                List<CommentTb>? model = await context.CommentTbs
                    .Where(m => m.VocTbId == vocid && m.DelYn != true)
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

        public async Task<CommentTb?> GetCommentInfo(int commentid)
        {
            try
            {
                CommentTb? model = await context.CommentTbs
                    .FirstOrDefaultAsync(m => m.Id == commentid && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
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
        /// 댓글 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateCommentInfo(CommentTb model)
        {
            try
            {
                context.CommentTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
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
                // 파일업로드 실패를 대비해서 Throw하면 실행이 종료되니 return false로 흐름을 이어감.
                return false;
                //throw new ArgumentNullException();
            }
        }
    }
}
