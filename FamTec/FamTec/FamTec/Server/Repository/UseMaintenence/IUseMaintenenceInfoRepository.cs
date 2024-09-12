using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;

namespace FamTec.Server.Repository.UseMaintenence
{
    public interface IUseMaintenenceInfoRepository
    {
        /// <summary>
        /// 사용자재 세부 이력 리스트
        /// </summary>
        /// <param name="useid"></param>
        /// <returns></returns>
        ValueTask<UseMaterialDetailDTO?> GetDetailUseStoreList(int usematerialid, int placeid);
        
        

    }
}
