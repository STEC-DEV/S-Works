using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocService
    {
        /// <summary>
        /// VOC 엑셀 양식 다운로드
        /// </summary>
        /// <returns></returns>
        public Task<byte[]?> DownloadVocForm(HttpContext context);

        /// <summary>
        /// 등록된 미누언 처리내역 최신상태 알림톡으로 전송
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> RecentVocSendService(HttpContext context, RecentVocDTO dto);

        /// <summary>
        /// 사업장별 VOC 월간 전체보기 [Regacy]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetMonthVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchDate);

        /// <summary>
        /// 사업장별 VOC 월간 전체보기 - V2
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocListDTOV2>> GetMonthVocSearchListV2(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchDate);


        /// <summary>
        /// 사업장별 VOC 기간 전체보기 [Regacy]
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetDateVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, DateTime StartDate, DateTime EndDate);

        /// <summary>
        /// 사업장별 VOC 기간 전체보기 - V2
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocListDTOV2>> GetDateVocSearchListV2(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, DateTime StartDate, DateTime EndDate);


        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<AllVocListDTO>> GetVocList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division);

        /// <summary>
        /// 조건별 민원 리스트 조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetVocFilterList(HttpContext context, DateTime startdate, DateTime enddate, List<int> type, List<int> status, List<int> buildingid, List<int> division);

        /// <summary>
        /// VOC 상세보기
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<VocEmployeeDetailDTO>> GetVocDetail(HttpContext context, int vocid, bool isMobile);

        /// <summary>
        /// VOC 유형 변경
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateVocTypeService(HttpContext context, UpdateVocDTO dto);

        /// <summary>
        /// DashBoard용 금일 처리유형별 발생건수
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<VocDaysStatusCountDTO>?> GetVocDaysStatusDataService(HttpContext context);

        /// <summary>
        /// DashBoard용 일주일치 처리유형별 발생건수
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocWeekStatusCountDTO>?> GetVocWeeksStatusDataService(HttpContext context);

        /// <summary>
        /// DashBoard용 하루치 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<VocDaysCountDTO>?> GetVocDashBoardDaysDataService(HttpContext context);
        

        /// <summary>
        /// DashBoard용 일주일치 민원 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<VocWeekCountDTO>?> GetVocDashBoardWeeksDataService(HttpContext context);
    }
}
