using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.Unit;

namespace FamTec.Server.Services.Unit
{
    public interface IUnitService
    {
        /// <summary>
        /// 해당 사업장의 단위리스트 조회
        /// </summary>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        ValueTask<ResponseList<UnitsDTO>?> GetUnitList(HttpContext? context);

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<UnitsDTO>> AddUnitService(HttpContext? context, UnitsDTO? dto);

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public ValueTask<ResponseModel<string>?> DeleteUnitService(UnitsDTO? dto, SessionInfo? sessioninfo);

    }
}
