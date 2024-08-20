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
        ValueTask<KakaoLogTb?> AddAsync(KakaoLogTb model);

        /// <summary>
        /// 카카오 알림톡 발송로그 리스트 전체조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<KakaoLogTb>?> GetKakaoLogList(int placeid);

        /// <summary>
        /// 카카오 알림톡 발송로그 기간별 조회
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        ValueTask<List<KakaoLogTb>?> GetKakaoLogList(DateTime StartDate, DateTime EndDate);
    }
}
