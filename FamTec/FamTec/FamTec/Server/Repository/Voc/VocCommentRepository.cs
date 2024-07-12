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

        public async ValueTask<CommentTb?> AddAsync(CommentTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.CommentTbs.Add(model);
                    await context.SaveChangesAsync();
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

        public async ValueTask<List<CommentTb>?> GetCommentList(int? vocid)
        {
            try
            {
                if(vocid is not null)
                {
                    List<CommentTb>? model = await context.CommentTbs.Where(m => m.VocTbId == vocid && m.DelYn != true).ToListAsync();

                    if (model is [_, ..])
                        return model;
                    else
                        return null;
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
    }
}
