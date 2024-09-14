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
        public ValueTask<ResponseUnit<UseMaterialDetailDTO>> GetDetailUseMaterialService(HttpContext context, int usematerialid);

        /// <summary>
        /// 사용자재 수정 서비스 - 추가출고 / 입고처리
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateDetailUseMaterialService(HttpContext context, UpdateMaintenanceMaterialDTO dto);
    }
}
