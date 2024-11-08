using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;

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
        public Task<AddKakaoLogDTO?> AddVocAnswer(string title, string receiptnum, DateTime receiptdate, string receiver, string url, string placetel);

        /// <summary>
        /// 민원인 전용 VOC 상태변경 메시지
        /// </summary>
        /// <param name="receiptnum">접수번호</param>
        /// <param name="status">진행상태</param>
        /// <param name="receiver">받는사람 전화번호</param>
        /// <param name="url">링크 URL</param>
        /// <param name="placetel">사업장 전화번호</param>
        /// <returns></returns>
        public Task<AddKakaoLogDTO?> UpdateVocAnswer(string receiptnum, string status, string receiver, string url, string placetel);

        /// <summary>
        /// 랜덤코드 생성
        /// </summary>
        /// <returns></returns>
        public string RandomCode();

        /// <summary>
        /// 인증코드 발급
        /// </summary>
        /// <returns></returns>
        public string RandomVerifyAuthCode();

        /// <summary>
        /// 인증코드 전송
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="authcode"></param>
        /// <returns></returns>
        public Task<bool> AddVerifyAuthCodeAnser(string buildingname, string phonenumber, string authcode);


        /// <summary>
        /// 카카오 전송결과 보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="StartDate"></param>
        /// <param name="limit_day"></param>
        /// <returns></returns>
        public Task<ResponseList<KaKaoSenderResult>?> KakaoSenderResult(HttpContext context, int page, int pagesize, DateTime StartDate, int limit_day);
    }
}
