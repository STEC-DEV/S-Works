using FamTec.Shared.Server.DTO.Voc;
using FamTec.Shared.Server.DTO;

namespace FamTec.Server.Services.Voc.Hub
{
    /// <summary>
    /// 민원인 전용
    /// </summary>
    public interface IHubService
    {
        
        /// <summary>
        /// 민원추가 - 민원인 전용
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddVocReturnDTO?>> AddVocService(AddVocDTO dto, List<IFormFile>? image);

        /// <summary>
        /// 민원 조회 - 민원인 전용
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocUserDetailDTO?>> GetVocRecord(string? voccode, bool isMobile);

        /// <summary>
        /// 민원 댓글조회 - 민원인 전용
        /// </summary>
        /// <param name="voccode"></param>
        /// <returns></returns>
        public Task<ResponseList<VocCommentListDTO>?> GetVocCommentList(string? voccode, bool isMobile);

        /// <summary>
        /// 민원 댓글상세조회 - 민원인 전용
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(int? commentid, bool isMobile);

    }
}
