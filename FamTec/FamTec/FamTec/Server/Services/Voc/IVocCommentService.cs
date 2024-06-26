using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocCommentService
    {
        /// <summary>
        /// 민원 댓글 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddVocCommentDTO>> AddVocCommentService(HttpContext? context, AddVocCommentDTO? dto);

        /// <summary>
        /// 해당 민원의 댓글 상세정보
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<VocCommentListDTO>> GetVocCommentList(int? vocid);
    }
}
