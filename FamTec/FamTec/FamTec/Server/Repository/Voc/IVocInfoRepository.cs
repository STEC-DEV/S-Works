using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Repository.Voc
{
    public interface IVocInfoRepository
    {
        /// <summary>
        /// VOC 추가
        /// </summary>
        /// <returns></returns>
        Task<VocTb?> AddAsync(VocTb model);

        /// <summary>
        /// 사업장별 민원리스트 조회
        /// </summary>
        /// <returns></returns>
        Task<List<AllVocListDTO>?> GetVocList(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division);

        /// <summary>
        /// 월간 사업장 VOC 조회 [Regacy]
        /// </summary>
        /// <returns></returns>
        Task<List<VocListDTO>?> GetVocMonthList(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division, int searchyear, int searchmonth);

        /// <summary>
        /// 월간 사업장 VOC 조회 - V2
        /// </summary>
        /// <returns></returns>
        Task<List<VocListDTOV2>?> GetVocMonthListV2(int placeid, List<int> type, List<int> status, List<int> buildingid, List<int> division, int searchyear, int searchmonth);

        /// <summary>
        /// 조건별 민원리스트 조회 [Regacy]
        /// </summary>
        /// <returns></returns>
        Task<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status, List<int> BuildingID, List<int> division);

        /// <summary>
        /// 조건별 민원리스트 조회 - V2
        /// </summary>
        /// <returns></returns>
        Task<List<VocListDTOV2>?> GetVocFilterListV2(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status, List<int> BuildingId, List<int> division);
            
        /// <summary>
        /// ID로 민원 상세조회
        /// </summary>
        /// <returns></returns>
        Task<VocTb?> GetVocInfoById(int vocid);

        /// <summary>
        /// VOC 코드로 단일모델 조회
        /// </summary>
        /// <returns></returns>
        Task<VocTb?> GetVocInfoByCode(string code);


        Task<bool> UpdateVocInfo(VocTb model);

        /// <summary>
        /// DashBoard 용 일주일치 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        Task<List<VocWeekCountDTO>?> GetDashBoardWeeksData(DateTime StartDate, DateTime EndDate, int placeid);

        /// <summary>
        /// DashBoard 용 오늘치 각 타입별 카운트
        /// </summary>
        /// <returns></returns>
        Task<VocDaysCountDTO?> GetDashBoardDaysData(DateTime NowDate, int placeid);

        /// <summary>
        /// DashBoard 용 7일 처리현황별 카운트
        /// </summary>
        /// <returns></returns>
        Task<List<VocWeekStatusCountDTO>?> GetDashBoardWeeksStatusData(DateTime StartDate, DateTime EndDate, int placeid);

        /// <summary>
        /// DashBoard 용 금일 처리현황별 카운트
        /// </summary>
        /// <returns></returns>
        Task<VocDaysStatusCountDTO?> GetDashBoardDaysStatusData(DateTime NowDate,int placeid);

    }
}
