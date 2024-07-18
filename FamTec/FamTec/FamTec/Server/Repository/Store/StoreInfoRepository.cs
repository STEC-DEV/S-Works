using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Store
{
    public class StoreInfoRepository : IStoreInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public StoreInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<StoreTb?> AddAsync(StoreTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.StoreTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 품목에 사용자재 List 출력
        /// </summary>
        /// <param name="Materialid"></param>
        /// <returns></returns>
        public async ValueTask<List<StoreListDTO>> UsedMaterialList(int? Materialid)
        {
            try
            {
                return null;
                //if(Materialid is not null)
                //{
                //    List<StoreListDTO> UsedList = (from Store in context.StoreTbs.Where(m => m.DE))
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public ValueTask<List<StoreTb>?> GetStoreList(int? placeid)
        {
            throw new NotImplementedException();
        }

     
    }
}
