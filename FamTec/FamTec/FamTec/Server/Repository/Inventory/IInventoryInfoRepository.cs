using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Inventory
{
    public interface IInventoryInfoRepository
    {
        ValueTask<InventoryTb?> AddAsync(InventoryTb? model);

        ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialId,int? delcount, string? GUID);

        ValueTask<InventoryTb?> GetInventoryInfo(int? id);

        ValueTask<bool?> AvailableCheck(int? placeid, int? roomid, int? materialid, string? guid);

        ValueTask<bool?> SetOutInventoryInfo(List<InventoryTb?> model, int delcount, string creater, string GUID);

        ValueTask<Task?> RoolBackOccupant(string GUID);

        
    }
}
