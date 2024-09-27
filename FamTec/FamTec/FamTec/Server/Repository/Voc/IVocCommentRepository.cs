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
        Task<CommentTb?> AddAsync(CommentTb model);

        /// <summary>
        /// VOCID에 해당하는 댓글 리스트 조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        Task<List<CommentTb>?> GetCommentList(int vocid);

        /// <summary>
        /// 댓글내용 상세보기
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        Task<CommentTb?> GetCommentInfo(int commentid);

        /// <summary>
        /// 댓글수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateCommentInfo(CommentTb model);

    }
}
