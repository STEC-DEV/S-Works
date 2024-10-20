using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Mvc;

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
        public Task<ResponseUnit<FailResult?>> AddMaintanceService(HttpContext context, AddMaintenanceDTO dto);

        /// <summary>
        /// 사용자재 추가출고
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<FailResult?>> AddSupMaintanceService(HttpContext context, AddMaintanceMaterialDTO dto);

        /// <summary>
        /// 유지보수 이미지 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> AddMaintanceImageService(HttpContext context, int id, IFormFile? files);

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name=""></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(HttpContext context, int facilityid);

        /// <summary>
        /// 유지보수용 출고내용 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteMaintenanceStoreRecordService(HttpContext context, DeleteMaintanceDTO dto);

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteMaintenanceRecordService(HttpContext context, DeleteMaintanceDTO2 dto);

        /// <summary>
        /// 유지보수 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateMaintenanceService(HttpContext context, UpdateMaintenanceDTO dto, IFormFile? files);

        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<ResponseList<MaintanceHistoryDTO>?> GetDateHisotryList(HttpContext context, DateTime StartDate, DateTime EndDate, List<string> category, List<int> type);

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<ResponseList<AllMaintanceHistoryDTO>?> GetAllHistoryList(HttpContext context, List<string> category, List<int> type);

        /// <summary>
        /// 설비의 유지보수리스트중 하나 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaintanceID"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DetailMaintanceDTO?>> GetDetailService(HttpContext context, int MaintanceID);
    }
}
