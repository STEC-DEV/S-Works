using FamTec.Server.Repository.Meter;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter;

namespace FamTec.Server.Services.Meter
{
    public class MeterService : IMeterService
    {
        private readonly IMeterInfoRepository MeterInfoRepository;

        private ILogService LogService;

        public MeterService(IMeterInfoRepository _meterinforepository, ILogService _logservice)
        {
            this.MeterInfoRepository = _meterinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 검침기 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddMeterDTO>> AddMeterService(HttpContext context, AddMeterDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddMeterDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddMeterDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                MeterItemTb model = new MeterItemTb
                {
                    Name = dto.Name!,
                    Category = dto.Category!,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    ContractTbId = dto.ContractTbId
                };

                return null;



            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddMeterDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
