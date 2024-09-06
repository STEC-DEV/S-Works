using FamTec.Shared.Model;

namespace FamTec.Server.Repository.UseMaintenence
{
    public interface IUseMaintenenceInfoRepository
    {

        ValueTask<UseMaintenenceMaterialTb?> GetUseMaintenenceInfo(int id);
        
        

    }
}
