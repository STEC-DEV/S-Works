using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Maintenance
{
    public interface IMaintanceService
    {
        /// <summary>
        /// DashBoard용 금일 유지보수 List
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintenanceDaysDTO>?> GetMaintenanceDaysList();

        /// <summary>
        /// DashBoard용 1년치 타입별 유지보수 금액
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceYearPriceDTO>?> GetMaintenanceYearPriceList();

        /// <summary>
        /// DashBoard용 일주일치 유지보수 카운트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="years"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceWeekCount>?> GetMaintanceDashBoardDataService();

        /// <summary>
        /// 유지보수 출고등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<FailResult?>> AddMaintanceService(AddMaintenanceDTO dto);

        /// <summary>
        /// 사용자재 추가출고
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<FailResult?>> AddSupMaintanceService(AddMaintanceMaterialDTO dto);

        /// <summary>
        /// 유지보수 이미지 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> AddMaintanceImageService(int id, IFormFile? files);

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name=""></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(int facilityid);

        /// <summary>
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteMaintenanceStoreRecordService(DeleteMaintanceDTO dto);

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteMaintenanceRecordService(DeleteMaintanceDTO2 dto);

        /// <summary>
        /// 유지보수 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateMaintenanceService(UpdateMaintenanceDTO dto, IFormFile? files);


        /// <summary>
        /// 속한 사업장 유지보수 월별 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchdate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceHistoryDTO>?> GetMonthHistoryList(string searchdate, List<string> category, List<int> type);

        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceHistoryDTO>?> GetDateHistoryList(DateTime StartDate, DateTime EndDate, List<string> category, List<int> type);

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<ResponseList<AllMaintanceHistoryDTO>?> GetAllHistoryList(List<string> category, List<int> type);

        /// <summary>
        /// 설비의 유지보수리스트중 하나 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaintanceID"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DetailMaintanceDTO?>> GetDetailService(int MaintanceID, bool isMobile);
    }
}
