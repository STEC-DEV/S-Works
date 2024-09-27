using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter;

namespace FamTec.Server.Services.Meter
{
    public interface IMeterService
    {
        /// <summary>
        /// 검침기 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddMeterDTO>> AddMeterService(HttpContext context, AddMeterDTO dto);

        /// <summary>
        /// 해당 사업장의 검침기 전체조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MeterDTO>> GetAllMeterListService(HttpContext context);

    }
}
