
using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Maintenence
{
    public interface IMaintenenceRepository
    {
        /// <summary>
        /// 유지보수이력 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<MaintenenceHistoryTb?> AddAsync(MaintenenceHistoryTb? model);

        /// <summary>
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        ValueTask<List<MaintenenceHistoryTb>?> GetFacilityHistoryList(int? facilityid);

        /// <summary>
        /// 유지보수 이력 사업장별 전체 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<MaintenenceHistoryTb>?> GetPlaceHistoryList(int? placeid);

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        ValueTask<List<MaintenenceHistoryTb>?> GetDateHistoryList(int? placeid, string? date);

        /// <summary>
        /// 유지보수이력 ID에 해당하는 상세정보 검색
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<MaintenenceHistoryTb>? GetDetailHistoryInfo(int? id);

        /// <summary>
        /// 유지보수이력 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<MaintenenceHistoryTb>? UpdateHistoryInfo(MaintenenceHistoryTb? model);

        /// <summary>
        /// 유지보수이력 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<MaintenenceHistoryTb>? DeleteHistoryInfo(MaintenenceHistoryTb? model);
    }
}
