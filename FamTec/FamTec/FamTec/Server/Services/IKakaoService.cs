namespace FamTec.Server.Services
{
    public interface IKakaoService
    {
        /// <summary>
        /// 민원인 전용 VOC 등록 확인메시지
        /// </summary>
        /// <returns></returns>
        public Task<bool?> AddVocAnswer(string title, string receiptnum, DateTime receiptdate, string reciver, string url, string placetel);

        /// <summary>
        /// 랜덤코드 생성
        /// </summary>
        /// <returns></returns>
        public string RandomCode();
    }
}
