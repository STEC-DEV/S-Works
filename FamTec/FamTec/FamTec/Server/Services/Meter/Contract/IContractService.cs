using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Meter.Contract;

namespace FamTec.Server.Services.Meter.Contract
{
    public interface IContractService
    {
        /// <summary>
        /// 계약종류 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddContractDTO>> AddContractService(HttpContext context, AddContractDTO dto);

        /// <summary>
        /// 계약종류 전체 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<ContractDTO>> GetAllContractListService(HttpContext context);

        
    }
}
