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
        /// 해당 사업장의 카카오 로그 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<KakaoLogListDTO>?> GetKakaoLogListService(HttpContext context);
    }
}
