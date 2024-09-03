using FamTec.Server.Repository.Meter.Energy;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Energy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace FamTec.Server.Services.Meter.Energy
{
    public class EnergyService : IEnergyService
    {
        private readonly IEnergyInfoRepository EnergyInfoRepository;
        private ILogService LogService;

        public EnergyService(IEnergyInfoRepository _energyinforepository, ILogService _logservice)
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
        public async ValueTask<ResponseUnit<AddEnergyDTO>> AddEnergyService(HttpContext context, AddEnergyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                EnergyUsageTb EnergyUserTB = new EnergyUsageTb()
                {
                    MeterDt = dto.MeterDate,
                    UseAmount = dto.UseAmount,
                    MeterItemId = dto.MeterID,
                    CreateDt = DateTime.Now,
                    CreateUser = creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = creater
                };

                EnergyUsageTb? model = await EnergyInfoRepository.AddAsync(EnergyUserTB);
                if (model is not null)
                    return new ResponseUnit<AddEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                else
                    return new ResponseUnit<AddEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 선택된 년도의 달의 일별 합산 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SearchDate"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<DayEnergyDTO>> GetMonthListService(HttpContext context, DateTime SearchDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<DayEnergyDTO>? model = await EnergyInfoRepository.GetMonthList(SearchDate, Int32.Parse(placeidx));
               
                if (model is not null)
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<DayEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
