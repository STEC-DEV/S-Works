using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Repository.Maintenence
{
    public interface IMaintanceRepository
    {
        /// <summary>
        /// 유지보수 ID로 유지보수 검색
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<MaintenenceHistoryTb> GetMaintenanceInfo(int id);

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool> UpdateMaintenanceInfo(MaintenenceHistoryTb model);

        /// <summary>
        /// 유지보수이력 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<int?> AddMaintanceAsync(AddMaintenanceDTO model, string creater, string userid, int placeid);

        /// <summary>
        /// 유지보수 이미지 추가
        /// </summary>
        /// <param name="id"></param>
        /// <param name="placeid"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        ValueTask<bool?> AddMaintanceImageAsync(int id, int placeid, IFormFile? files);

        /// <summary>
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        ValueTask<List<MaintanceListDTO>?> GetFacilityHistoryList(int facilityid, int placeid);

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        ValueTask<List<MaintanceHistoryDTO>?> GetDateHistoryList(int placeid, DateTime StartDate, DateTime EndDate, string Category, int type);

        /// <summary>
        /// 유지보수 이력 사업장별 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="Category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ValueTask<List<AllMaintanceHistoryDTO>?> GetAllHistoryList(int placeid, string Category, int type);

        /// <summary>
        /// 유지보수 상세조회
        /// </summary>
        /// <param name="MaintanceID"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<DetailMaintanceDTO?> DetailMaintanceList(int MaintanceID, int placeid);

        

        /// <summary>
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> deleteMaintenanceStoreRecord(List<DeleteMaintanceDTO> DeleteDTO, int placeid, string deleter);

        /// <summary>
        /// 유지보수 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        ValueTask<bool?> deleteMaintenanceRecord(DeleteMaintanceDTO2 dto, int placeid, string deleter);

      
    }
}
