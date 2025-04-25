using FamTec.Server.Helpers;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocCommentService
    {
        /// <summary>
        /// 민원 댓글 추가 - Ver2
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<AddVocCommentDTOV2?>> AddVocCommentServiceV2(AddVocCommentDTOV2 dto, List<IFormFile> files);


        /// <summary>
        /// 민원 댓글 추가 [Regacy]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<AddVocCommentDTO?>> AddVocCommentService(AddVocCommentDTO dto, List<IFormFile> files);

        /// <summary>
        /// 해당 민원에 대한 댓글 리스트 조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<VocCommentListDTO>>> GetVocCommentList(int vocid, bool isMobile);

        /// <summary>
        /// 해당 민원에 대한 댓글 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<ResponseModel<VocCommentDetailDTO?>> GetVocCommentDetail(int commentid, bool isMobile);

        /// <summary>
        /// VOC 댓글 자기것만 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateCommentService(VocCommentDetailDTO dto, List<IFormFile>? files);

    }
}
