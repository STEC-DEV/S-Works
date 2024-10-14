using FamTec.Server.Repository.Meter.Energy;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Energy;

namespace FamTec.Server.Services.Meter.Energy
{
    public class EnergyService : IEnergyService
    {
        private readonly IEnergyInfoRepository EnergyInfoRepository;
        private ILogService LogService;

        public EnergyService(IEnergyInfoRepository _energyinforepository,
            ILogService _logservice)
        {
            this.EnergyInfoRepository = _energyinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 검침값 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<AddEnergyDTO>> AddEnergyService(HttpContext context, AddEnergyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                EnergyDayUsageTb? AlreadyCheck = await EnergyInfoRepository.GetUsageDaysInfo(dto.MeterID, dto.MeterDate.Year, dto.MeterDate.Month, dto.MeterDate.Day, Convert.ToInt32(placeidx));
                if (AlreadyCheck is not null)
                    return new ResponseUnit<AddEnergyDTO>() { message = "이미 값이 있습니다.", data = null, code = 200 };

                EnergyDayUsageTb EnergyUserTB = new EnergyDayUsageTb()
                {
                    MeterDt = dto.MeterDate,
                    Amount1 = dto.Amount1,
                    Amount2 = dto.Amount2,
                    Amount3 = dto.Amount3,
                    TotalAmount = dto.TotalAmount,
                    Year = dto.MeterDate.Year,
                    Month = dto.MeterDate.Month,
                    Days = dto.MeterDate.Day,
                    CreateDt = DateTime.Now,
                    CreateUser = creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = creater,
                    MeterItemId = dto.MeterID,
                    PlaceTbId = Convert.ToInt32(placeidx)
                };

                EnergyDayUsageTb? model = await EnergyInfoRepository.AddAsync(EnergyUserTB);
                if (model is not null)
                    return new ResponseUnit<AddEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

   

     

    }
}
