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
        public ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext context, AddMaintanceDTO dto);

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name=""></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(HttpContext context, int facilityid);

        /// <summary>
        /// 해당 설비의 유지보수 이력 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeletemaintanceHistoryService(HttpContext context, DeleteMaintanceDTO dto);
    }
}
