using FamTec.Shared.Server.DTO;

namespace FamTec.Server.Services
{
    public interface IKakaoService
    {
        /// <summary>
        /// 민원인 전용 VOC 등록 확인메시지
        /// </summary>
        /// <param name="title">제목</param>
        /// <param name="receiptnum">접수번호</param>
        /// <param name="receiptdate">접수일자</param>
        /// <param name="receiver">받는사람 전화번호</param>
        /// <param name="url">링크 URL</param>
        /// <param name="placetel">사업장 전화번호</param>
        /// <returns></returns>
        public Task<KakaoLogDTO?> AddVocAnswer(string title, string receiptnum, DateTime receiptdate, string receiver, string url, string placetel);

        /// <summary>
        /// 민원인 전용 VOC 상태변경 메시지
        /// </summary>
        /// <param name="receiptnum">접수번호</param>
        /// <param name="status">진행상태</param>
        /// <param name="receiver">받는사람 전화번호</param>
        /// <param name="url">링크 URL</param>
        /// <param name="placetel">사업장 전화번호</param>
        /// <returns></returns>
        public Task<KakaoLogDTO?> UpdateVocAnswer(string receiptnum, string status, string receiver, string url, string placetel);

        /// <summary>
        /// 랜덤코드 생성
        /// </summary>
        /// <returns></returns>
        public string RandomCode();
    }
}
