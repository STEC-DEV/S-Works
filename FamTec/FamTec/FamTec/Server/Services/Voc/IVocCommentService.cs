using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Http;

namespace FamTec.Server.Services.Voc
{
    public interface IVocCommentService
    {
        /// <summary>
        /// 민원 댓글 추가 - Ver2
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<AddVocCommentDTOV2?>> AddVocCommentServiceV2(HttpContext context, AddVocCommentDTOV2 dto, List<IFormFile> files);


        /// <summary>
        /// 민원 댓글 추가 [Regacy]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<AddVocCommentDTO?>> AddVocCommentService(HttpContext context, AddVocCommentDTO dto, List<IFormFile> files);

        /// <summary>
        /// 해당 민원에 대한 댓글 리스트 조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public Task<ResponseList<VocCommentListDTO>> GetVocCommentList(HttpContext context, int vocid, bool isMobile);

        /// <summary>
        /// 해당 민원에 대한 댓글 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(HttpContext context, int commentid, bool isMobile);

        /// <summary>
        /// VOC 댓글 자기것만 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateCommentService(HttpContext context, VocCommentDetailDTO dto, List<IFormFile>? files);

    }
}
