using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;

namespace FamTec.Server.Services.KakaoLog
{
    /// <summary>
    ///  카카오 로그 서비스 추가해야함.
    /// </summary>
    public interface IKakaoLogService
    {

        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 기간 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public Task<ResponseList<KakaoLogListDTO>> GetKakaoLogDateListService(HttpContext context, DateTime StartDate, DateTime EndDate, int isSuccess);

        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public Task<ResponseList<KakaoLogListDTO>> GetKakaoLogListService(HttpContext context, int isSuccess);

        /// <summary>
        /// 사업장 카카오 로그 리스트 카운트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseUnit<int?>> GetKakaoLogCountService(HttpContext context);

        /// <summary>
        /// 사업장에 속해있는 카카오 로그 리스트 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Task<ResponseList<KakaoLogListDTO>> GetKakaoLogPageNationListService(HttpContext context, int pagenumber, int pagesize);

    }
}
