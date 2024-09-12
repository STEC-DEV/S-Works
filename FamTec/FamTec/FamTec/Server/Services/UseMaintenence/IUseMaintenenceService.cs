using FamTec.Shared.Server.DTO;
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
    }
}
