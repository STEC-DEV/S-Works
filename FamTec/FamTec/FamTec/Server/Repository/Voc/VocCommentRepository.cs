using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Voc
{
    public class VocCommentRepository : IVocCommentRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public VocCommentRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원에 댓글 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<CommentTb?> AddAsync(CommentTb model)
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }



        public async ValueTask<List<CommentTb>?> GetCommentList(int vocid)
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }


        public async ValueTask<CommentTb?> GetCommentInfo(int commentid)
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }


        /// <summary>
        /// 댓글 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateCommentInfo(CommentTb model)
        {
            try
            {
                context.CommentTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());

                // 파일업로드 실패를 대비해서 Throw하면 실행이 종료되니 return false로 흐름을 이어감.
                return false;
                //throw new ArgumentNullException();
            }
        }
    }
}
