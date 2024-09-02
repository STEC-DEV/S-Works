using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Meter.Contract
{
    public interface IContractInfoRepository
    {
        /// <summary>
        /// 계약종 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<ContractTypeTb?> AddAsync(ContractTypeTb model);

        /// <summary>
        /// 사업장에 속한 계약종류 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<List<ContractTypeTb>?> GetAllContractList(int placeid);

        /// <summary>
        /// 계약명칭으로 검색
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ValueTask<ContractTypeTb?> GetContractName(int placeid, string name);

        /// <summary>
        /// 계약종류 ID에 해당하는 계약종류정보 반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ValueTask<ContractTypeTb?> GetContractInfo(int id);
    }
}
