using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Voc
{
    public interface IVocCommentRepository
    {
        /// <summary>
        /// 민원에 댓글 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<CommentTb?> AddAsync(CommentTb? model);

        ValueTask<List<CommentTb>?> GetCommentList(int? vocid);


    }
}
