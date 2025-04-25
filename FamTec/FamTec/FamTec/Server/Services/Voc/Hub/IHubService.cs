using FamTec.Shared.Server.DTO.Voc;
using FamTec.Server.Helpers;

namespace FamTec.Server.Services.Voc.Hub
{
    /// <summary>
    /// 민원인 전용
    /// </summary>
    public interface IHubService
    {

        /// <summary>
        /// 인증코드 발급
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<bool>> AddAuthCodeService(int PlaceId, int BuildingId, string PhoneNumber);

        /// <summary>
        /// 인증코드 검사
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<bool>> GetVerifyAuthCodeService(string PhoneNumber, string AuthCode);

        /// <summary>
        /// 민원추가 Version2 
        /// - 알림톡 전송여부 기능추가
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<AddVocReturnDTO?>> AddVocServiceV2(AddVocDTOV2 dto, List<IFormFile>? image);

        /// <summary>
        /// 민원추가 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<AddVocReturnDTO?>> AddVocService(AddVocDTO dto, List<IFormFile>? image);

        /// <summary>
        /// 민원 조회 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<VocUserDetailDTO?>> GetVocRecord(string? voccode, bool isMobile);

        /// <summary>
        /// 민원 댓글조회 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<List<VocCommentListDTO>>?> GetVocCommentList(string? voccode, bool isMobile);

        /// <summary>
        /// 민원 댓글상세조회 - 민원인 전용
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<VocCommentDetailDTO?>> GetVocCommentDetail(int? commentid, bool isMobile);

    }
}
