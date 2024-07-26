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

        /// <summary>
        /// 기간별 - 품목별 입출고 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        ValueTask<List<PeriodicInventoryRecordDTO>?> GetInventoryRecord(int? placeid, int? materialid, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// 품목별 창고별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="MaterialId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ValueTask<List<MaterialHistory>?> GetPlaceInventoryRecord(int? placeid, List<int>? MaterialId, bool? type);
    }
}
