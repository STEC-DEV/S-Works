using FamTec.Server.Helpers;
using FamTec.Shared.Server.DTO.Meter.Energy;

namespace FamTec.Server.Services.Meter.Energy
{
    public interface IEnergyService
    {
        /// <summary>
        /// 에너지 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<AddEnergyDTO>> AddEnergyService(AddEnergyDTO dto);

  

    }
}
