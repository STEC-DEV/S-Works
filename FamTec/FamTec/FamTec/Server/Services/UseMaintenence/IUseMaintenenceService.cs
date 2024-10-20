using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;

namespace FamTec.Server.Services.UseMaintenence
{
    public interface IUseMaintenenceService
    {
        /// <summary>
        /// 사용자재 상세 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<UseMaterialDetailDTO>> GetDetailUseMaterialService(HttpContext context, int usematerialid, int materialid, int roomid);

        /// <summary>
        /// 사용자재 수정 서비스 - 추가출고 / 입고처리
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateDetailUseMaterialService(HttpContext context, UpdateMaintenanceMaterialDTO dto);

        /// <summary>
        /// 사용자재 수정 서비스 - 추가출고 / 입고 / 생출고 / 삭제 로직처리
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateUseMaintanceService(HttpContext context, UpdateMaintancematerialDTO dto);
    }
}
