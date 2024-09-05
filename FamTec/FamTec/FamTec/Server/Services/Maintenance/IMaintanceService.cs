using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Services.Maintenance
{
    public interface IMaintanceService
    {
        /// <summary>
        /// 유지보수 출고등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext context, AddMaintenanceDTO dto, IFormFile? files);

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name=""></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(HttpContext context, int facilityid);

        /// <summary>
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteMaintenanceStoreRecordService(HttpContext context, List<DeleteMaintanceDTO> dto);
        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaintanceHistoryDTO>?> GetDateHisotryList(HttpContext context, DateTime StartDate, DateTime EndDate, string category, int type);

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<AllMaintanceHistoryDTO>?> GetAllHistoryList(HttpContext context, string category, int type);

        /// <summary>
        /// 설비의 유지보수리스트중 하나 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaintanceID"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<DetailMaintanceDTO?>> GetDetailService(HttpContext context, int MaintanceID);
    }
}
