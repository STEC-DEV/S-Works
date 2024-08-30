using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Repository.Maintenence
{
    public interface IMaintanceRepository
    {
        /// <summary>
        /// 유지보수이력 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //ValueTask<bool?> AddMaintanceAsync(AddMaintanceDTO model, string creater, int placeid, string GUID);
        ValueTask<bool?> AddMaintanceAsync(AddMaintanceDTO model, string creater, int placeid);

        /// <summary>
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        ValueTask<List<MaintanceListDTO>?> GetFacilityHistoryList(int facilityid);

        /// <summary>
        /// 유지보수이력 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteHistoryInfo(DeleteMaintanceDTO DeleteDTO, string deleter);

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

    }
}
