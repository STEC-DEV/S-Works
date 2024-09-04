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
            if (context is null)
                return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

            string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

            if (String.IsNullOrWhiteSpace(placeidx))
                return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

            List<DayEnergyDTO>? model = await EnergyInfoRepository.GetMonthList(SearchDate, Int32.Parse(placeidx));

            if (model is not null)
                return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            else
                return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
        }

        /// <summary>
        /// 선택된 년도의 선택된 검침기의 달의 일별 합산 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SearchDate"></param>
        /// <param name="MeterId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseList<DayEnergyDTO>> GetMonthSelectListService(HttpContext context, DateTime SearchDate, List<int> MeterId)
        {
            try
            {
                if (context is null)
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<DayEnergyDTO>? model = await EnergyInfoRepository.GetMeterMonthList(SearchDate, MeterId, Int32.Parse(placeidx));

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

        /// <summary>
        /// 선택된 년도의 월별 통계
        /// </summary>
        /// <param name="context"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<YearsTotalEnergyDTO>> GetYearListService(HttpContext context, int year)
        {
            try
            {
                if (context is null)
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<YearsTotalEnergyDTO>? model = await EnergyInfoRepository.GetYearsList(year, Int32.Parse(placeidx));
                
                if (model is not null && model.Any())
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<YearsTotalEnergyDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<YearsTotalEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 선택된 년도의 선택된 검침기의 월별 통계
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MeterId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<YearsTotalEnergyDTO>> GetYearSelectListService(HttpContext context, List<int> MeterId, int year)
        {
            try
            {
                if (context is null)
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<YearsTotalEnergyDTO>? model = await EnergyInfoRepository.GetMeterYearsList(year, MeterId, Int32.Parse(placeidx));

                if (model is not null && model.Any())
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<YearsTotalEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<YearsTotalEnergyDTO>(), code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<YearsTotalEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 선택된 일자 사이의 데이터 리스트 전체출력
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<DayEnergyDTO>> GetDaysListService(HttpContext context, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<DayEnergyDTO>? model = await EnergyInfoRepository.GetDaysList(StartDate, EndDate, Int32.Parse(placeidx));

                if (model is not null)
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<DayEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 선택된 검침기의 선택된 일자 사이의 데이터 리스트 출력 - 선택
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MeterId"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<DayEnergyDTO>> GetDaysSelectListService(HttpContext context, List<int> MeterId, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<DayEnergyDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<DayEnergyDTO>? model = await EnergyInfoRepository.GetMeterDaysList(StartDate, EndDate, MeterId, Int32.Parse(placeidx));

                if (model is not null)
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<DayEnergyDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<DayEnergyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
