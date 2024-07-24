using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Inventory
{
    public interface IInventoryInfoRepository
    {
        ValueTask<bool?> AddAsync(List<InOutInventoryDTO>? model, string? creater, int? placeid, string? GUID);

        ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialid,int? delcount, string? GUID);

        ValueTask<InventoryTb?> GetInventoryInfo(int? id);

        ValueTask<bool?> SetOccupantToken(int? placeid, int? roomid, int? materialid, string? guid);

        ValueTask<bool?> SetOutInventoryInfo(List<InOutInventoryDTO?> model, string? creater, int? placeid, string? GUID);
        //ValueTask<bool?> SetOutInventoryInfo(List<InventoryTb?> model, int delcount, string creater, string GUID);

        ValueTask<Task?> RoolBackOccupant(string GUID);

        ValueTask<bool?> AvailableCheck(int? placeid, int? roomid, int? materialid);

        ValueTask<bool?> GetInventoryRecord(int? placeid, int? materialid);

        ValueTask<bool?> GetInventoryRecord2(int? placeid);
    }
}
