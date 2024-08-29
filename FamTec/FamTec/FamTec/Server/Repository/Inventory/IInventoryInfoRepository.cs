using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Repository.Inventory
{
    public interface IInventoryInfoRepository
    {
        /// <summary>
        /// 입고등록
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        ValueTask<bool?> AddAsync(List<InOutInventoryDTO> model, string creater, int placeid, string GUID);

        ValueTask<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid,int delcount, string GUID);

        /// <summary>
        /// 출고등록
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        ValueTask<bool?> SetOutInventoryInfo(List<InOutInventoryDTO> model, string creater, int placeid, string GUID);

        /// <summary>
        /// 동시성 토큰 등록
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="dto"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ValueTask<bool?> SetOccupantToken(int placeid, List<InOutInventoryDTO> dto, string guid);

        /// <summary>
        /// 롤백
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        ValueTask<Task?> RoolBackOccupant(string GUID);

        /// <summary>
        /// 기간별 - 품목별 입출고 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        ValueTask<List<PeriodicInventoryRecordDTO>?> GetInventoryRecord(int placeid, int materialid, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 품목별 창고별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="MaterialId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ValueTask<List<MaterialHistory>?> GetPlaceInventoryRecord(int placeid, List<int> MaterialId, bool type);

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 재고리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        ValueTask<List<InventoryTb>?> GetPlaceMaterialInventoryList(int placeid, int materialid);

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 위치 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        ValueTask<List<InOutLocationDTO>> GetLocationMaterialInventoryList(int placeid, int materialid);

        ValueTask<bool?> AddOutStoreList(int placeid, int roomid, int materialid, int outcount);
    }
}
