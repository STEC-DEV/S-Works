using FamTec.Shared.Server.DTO.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;

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
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> AddAuthCodeService(int PlaceId, int BuildingId, string PhoneNumber);

        /// <summary>
        /// 인증코드 검사
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="authcode"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> GetVerifyAuthCodeService(string PhoneNumber, string AuthCode);


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
