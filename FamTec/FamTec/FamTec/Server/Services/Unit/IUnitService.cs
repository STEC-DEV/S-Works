using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Unit;

namespace FamTec.Server.Services.Unit
{
    public interface IUnitService
    {
        /// <summary>
        /// 해당 사업장의 단위리스트 조회
        /// </summary>
        /// <returns></returns>
        ValueTask<ResponseList<UnitsDTO>> GetUnitList(HttpContext context);

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<UnitsDTO>> AddUnitService(HttpContext context, UnitsDTO dto);

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteUnitService(HttpContext context, List<int> unitid);

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<UnitsDTO>> UpdateUnitService(HttpContext context, UnitsDTO dto);

    }
}
