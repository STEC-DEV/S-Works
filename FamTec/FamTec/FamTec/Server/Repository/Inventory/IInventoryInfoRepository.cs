using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Inventory
{
    public interface IInventoryInfoRepository
    {
        ValueTask<InventoryTb?> AddAsync(InventoryTb? model);
        
        ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialId);

        ValueTask<InventoryTb?> GetInventoryInfo(int? id);

        ValueTask<bool?> AvailableCheck(int? placeid, int? roomid, int? materialid, string? guid);

    }
}
