using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;

namespace FamTec.Server.Repository.UseMaintenence
{
    public class UseMaintenenceInfoRepository : IUseMaintenenceInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public UseMaintenenceInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

       

        public ValueTask<UseMaintenenceMaterialTb?> GetUseMaintenenceInfo(int id)
        {
            throw new NotImplementedException();
        }
    }
}
