using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Meter.Contract
{
    public class ContractInfoRepository : IContractInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogServce;

        public ContractInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogServce = _logservice;
        }


        /// <summary>
        /// 계약종류 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<ContractTypeTb?> AddAsync(ContractTypeTb model)
        {
            try
            {
                await context.ContractTypeTbs.AddAsync(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;

                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogServce.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사업장에 속한 계약종류 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<ContractTypeTb>?> GetAllContractList(int placeid)
        {
            try
            {
                List<ContractTypeTb>? model = await context.ContractTypeTbs
                    .Where(m => m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .ToListAsync();

                if (model is not null && model.Any())
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogServce.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 계약 명칭으로 검색
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async ValueTask<ContractTypeTb?> GetContractName(int placeid, string name)
        {
            try
            {
                ContractTypeTb? model = await context.ContractTypeTbs
                    .FirstOrDefaultAsync(m => m.PlaceTbId == placeid &&
                                              m.DelYn != true &&
                                              m.Name == name);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogServce.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 계약종류 ID에 해당하는 계약종류정보 반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<ContractTypeTb?> GetContractInfo(int id)
        {
            try
            {
                ContractTypeTb? model = await context.ContractTypeTbs
                    .FirstOrDefaultAsync(m => m.Id == id && 
                                              m.DelYn != true);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogServce.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

  
    }
}
