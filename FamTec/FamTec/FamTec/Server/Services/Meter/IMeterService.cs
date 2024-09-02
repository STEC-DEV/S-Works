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
        public ValueTask<ResponseUnit<AddMeterDTO>> AddMeterService(HttpContext context, AddMeterDTO dto);
    }
}
