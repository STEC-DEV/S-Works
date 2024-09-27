using FamTec.Server.Repository.Meter.Contract;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Contract;

namespace FamTec.Server.Services.Meter.Contract
{
    public class ContractService : IContractService
    {
        private readonly IContractInfoRepository ContractInfoRepository;

        private ILogService LogService;

        public ContractService(IContractInfoRepository _contractinforepository,
            ILogService _logservice)
        {
            this.ContractInfoRepository = _contractinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 계약종류 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddContractDTO>> AddContractService(HttpContext context, AddContractDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddContractDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);

                if(String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddContractDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                ContractTypeTb? ContractInfo = await ContractInfoRepository.GetContractName(Int32.Parse(placeidx), dto.Name!);
                if (ContractInfo is not null)
                    return new ResponseUnit<AddContractDTO>() { message = "이미 존재하는 계약종류 입니다.", data = null, code = 201 };

                ContractTypeTb model = new ContractTypeTb
                {
                    Name = dto.Name!,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    PlaceTbId = Convert.ToInt32(placeidx)
                };

                ContractTypeTb? AddInfo = await ContractInfoRepository.AddAsync(model);
                if(AddInfo is not null)
                {
                    return new ResponseUnit<AddContractDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddContractDTO() { Name = AddInfo.Name }, code = 200 };
                }
                else
                    return new ResponseUnit<AddContractDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddContractDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500};
            }
        }

        /// <summary>
        /// 계약종류 전체 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<ContractDTO>> GetAllContractListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<ContractDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<ContractDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<ContractTypeTb>? model = await ContractInfoRepository.GetAllContractList(Int32.Parse(placeidx));
                if(model is not null && model.Any())
                {
                    List<ContractDTO>? dto = model.Select(e => new ContractDTO
                    {
                        ID = e.Id,
                        Name = e.Name
                    }).ToList();

                    return new ResponseList<ContractDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseList<ContractDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<ContractDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ContractDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
