using FamTec.Server.Repository.Meter;
using FamTec.Server.Repository.Meter.Contract;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter;

namespace FamTec.Server.Services.Meter
{
    public class MeterService : IMeterService
    {
        private readonly IMeterInfoRepository MeterInfoRepository;
        private readonly IContractInfoRepository ContractInfoRepository;

        private ILogService LogService;


        public MeterService(IMeterInfoRepository _meterinforepository, 
            IContractInfoRepository _contractinforepository,
            ILogService _logservice)
        {
            this.MeterInfoRepository = _meterinforepository;
            this.ContractInfoRepository = _contractinforepository;
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

                // 여기서 ADD 중복체크
                MeterItemTb? MeterInfo = await MeterInfoRepository.GetMeterName(Convert.ToInt32(placeidx), dto.Name!);
                if(MeterInfo is not null)
                    return new ResponseUnit<AddMeterDTO>() { message = "이미 존재하는 검침기명 입니다.", data = null, code = 201 };

                MeterItemTb model = new MeterItemTb
                {
                    Name = dto.Name!,
                    Category = dto.Category!,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    ContractTbId = dto.ContractTbId,
                    PlaceTbId = Convert.ToInt32(placeidx)
                };

                MeterItemTb? AddMeterResult = await MeterInfoRepository.AddAsync(model);
                if (AddMeterResult is not null)
                    return new ResponseUnit<AddMeterDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddMeterDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddMeterDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장의 검침기 전체조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MeterDTO>> GetAllMeterListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MeterDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<MeterDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MeterItemTb>? MeterList = await MeterInfoRepository.GetAllMeterList(Convert.ToInt32(placeidx));
                if(MeterList is not null && MeterList.Any())
                {
                    List<ContractTypeTb>? ContractList = await ContractInfoRepository.GetAllContractList(Convert.ToInt32(placeidx));
                    if(ContractList is not null && ContractList.Any())
                    {
                        List<MeterDTO> dto = (from Meter in MeterList
                                              join Contract in ContractList
                                              on Meter.ContractTbId equals Contract.Id
                                              select new MeterDTO
                                              {
                                                  Id = Meter.MeterItemId,
                                                  Category = Meter.Category,
                                                  Name = Meter.Name,
                                                  ContractId = Contract.Id,
                                                  ContractName = Contract.Name
                                              }).ToList();

                        return new ResponseList<MeterDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

                    }
                    else
                    {
                        List<MeterDTO> dto = MeterList.Select(e => new MeterDTO
                        {
                            Id = e.MeterItemId,
                            Category = e.Category,
                            Name = e.Name
                        }).ToList();

                        return new ResponseList<MeterDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                }
                else
                {
                    return new ResponseList<MeterDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<MeterDTO>(), code = 200 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MeterDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }

        }
    }
}
