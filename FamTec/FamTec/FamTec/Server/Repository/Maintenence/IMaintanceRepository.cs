using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Maintenence
{
    public interface IMaintanceRepository
    {

        /// <summary>
        /// 대쉬보드용 금일 유지보수 이력 리스트
        /// </summary>
        /// <param name="NowDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<MaintenanceDaysDTO>?> GetMaintenanceDaysData(DateTime NowDate, int placeid);

        Task<List<MaintanceWeekCount>?> GetMaintanceDashBoardData(DateTime startOfWeek, DateTime EndOfWeek, int placeid);

        /// <summary>
        /// 대쉬보드용 1년치 유지보수비용 조회
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="LastDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<MaintanceYearPriceDTO>?> GetMaintenanceYearData(DateTime StartDate, DateTime LastDate, int placeid);

        /// <summary>
        /// 유지보수 ID로 유지보수 검색
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MaintenenceHistoryTb?> GetMaintenanceInfo(int id);

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateMaintenanceInfo(MaintenenceHistoryTb model);

        /// <summary>
        /// 유지보수이력 추가 - 자체작업
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<FailResult?> AddSelfMaintanceAsync(AddMaintenanceDTO model, string creater, string userid, int placeid);

        /// <summary>
        /// 유지보수이력 추가 - 외주작업
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="creater"></param>
        /// <param name="userid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<FailResult?> AddOutSourcingMaintanceAsync(AddMaintenanceDTO dto, string creater, string userid, int placeid);

        /// <summary>
        /// 유지보수 사용자재 추가 출고
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<FailResult?> AddMaintanceMaterialAsync(AddMaintanceMaterialDTO model, string creater, int placeid);

        /// <summary>
        /// 유지보수 이미지 추가
        /// </summary>
        /// <param name="id"></param>
        /// <param name="placeid"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        Task<bool?> AddMaintanceImageAsync(int id, int placeid, IFormFile? files);

        /// <summary>
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        Task<List<MaintanceListDTO>?> GetFacilityHistoryList(int facilityid, int placeid);


        /// <summary>
        /// 유지보수 이력 사업장별 월별 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="Category"></param>
        /// <param name="type"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<MaintanceHistoryDTO>?> GetMonthHistoryList(int placeid, List<string> Category, List<int> type, int year, int month);

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<List<MaintanceHistoryDTO>?> GetDateHistoryList(int placeid, DateTime StartDate, DateTime EndDate, List<string> Category, List<int> type);

        /// <summary>
        /// 유지보수 이력 사업장별 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="Category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<AllMaintanceHistoryDTO>?> GetAllHistoryList(int placeid, List<string> Category, List<int> type);

        /// <summary>
        /// 기존건 유지보수 상세조회
        /// </summary>
        /// <param name="MaintanceID"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<DetailMaintanceDTO?> DetailMaintanceList(int MaintanceID, int placeid, bool isMobile);

       

        /// <summary>
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> deleteMaintenanceStoreRecord(DeleteMaintanceDTO DeleteDTO, int placeid, string deleter);

        /// <summary>
        /// 유지보수 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> deleteMaintenanceRecord(DeleteMaintanceDTO2 dto, int placeid, string deleter);


    }
}
