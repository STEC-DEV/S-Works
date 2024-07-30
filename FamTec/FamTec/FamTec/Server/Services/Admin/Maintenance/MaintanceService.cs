using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Maintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Services.Admin.Maintenance
{
    public class MaintanceService : IMaintanceService
    {
        private readonly IMaintanceRepository MaintanceRepository;
        private ILogService LogService;

        public MaintanceService(IMaintanceRepository _maintancerepository,
            ILogService _logservice)
        {
            this.MaintanceRepository = _maintancerepository;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext? context, AddMaintanceDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? GUID = Guid.NewGuid().ToString();

                bool? SetOccupantResult = await MaintanceRepository.SetOccupantToken(Convert.ToInt32(placeid), dto, GUID);
                if (SetOccupantResult == false)
                {
                    // 다른곳에서 사용중인 품목
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "다른곳에서 이미 사용중인 품목입니다.", data = null, code = 200 };
                }
                if (SetOccupantResult == null)
                {
                    // 조회결과가 없을때
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "조회결과가 없습니다.", data = null, code = 200 };
                }

                bool? OutResult = await MaintanceRepository.AddMaintanceAsync(dto, creater, Convert.ToInt32(placeid), GUID);
                if (OutResult == true)
                {
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if (OutResult == false)
                {
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
