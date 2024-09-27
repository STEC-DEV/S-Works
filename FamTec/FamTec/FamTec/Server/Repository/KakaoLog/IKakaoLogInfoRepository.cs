using FamTec.Shared.Model;

namespace FamTec.Server.Repository.KakaoLog
{
    public interface IKakaoLogInfoRepository
    {
        /// <summary>
        /// 카카오 알림톡 발송로그 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<KakaoLogTb?> AddAsync(KakaoLogTb model);

        /// <summary>
        /// 카카오 알림톡 발송로그 리스트 전체조회
        /// </summary>
        /// <returns></returns>
        Task<List<KakaoLogTb>?> GetKakaoLogList(int placeid);

        /// <summary>
        /// 사업장에 속해있는 카카오 알림톡 발송로그 카운트 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<int?> GetKakaoLogCount(int placeid);

        /// <summary>
        /// 카카오 알림톡 발송로그 리스트 페이지네이션 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<List<KakaoLogTb>?> GetKakaoLogPageNationList(int placeid, int pagenumber, int pagesize);

        /// <summary>
        /// 카카오 알림톡 발송로그 기간별 조회
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        Task<List<KakaoLogTb>?> GetKakaoLogList(DateTime StartDate, DateTime EndDate);
    }
}
