using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
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
        Task<int?> AddAsync(List<InOutInventoryDTO> model, string creater, int placeid);

        Task<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid,int delcount);

        /// <summary>
        /// 이월재고 수량
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        Task<int> GetCarryOverNum(int placeid, int materialid, DateTime StartDate);

        /// <summary>
        /// 출고등록
        /// </summary>
        /// <param name="model"></param>
        /// <param name="creater"></param>
        /// <param name="placeid"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        Task<FailResult?> SetOutInventoryInfo(List<InOutInventoryDTO> model, string creater, int placeid);

        /// <summary>
        /// 기간별 - 품목별 입출고 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<PeriodicDTO>?> GetInventoryRecord(int placeid, List<int> materialid, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 품목별 창고별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="MaterialId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<MaterialHistory>?> GetPlaceInventoryRecord(int placeid, List<int> MaterialId, bool type);

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 재고리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        Task<List<InventoryTb>?> GetPlaceMaterialInventoryList(int placeid, int materialid);

        /// <summary>
        /// 사업장 - 품목ID에 해당하는 위치 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        Task<List<InOutLocationDTO>> GetLocationMaterialInventoryList(int placeid, int materialid, int buildingid);


        /// <summary>
        /// 사업장 - 품목ID - 공간ID헤 대항하는 재고수량 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        Task<InOutLocationDTO?> GetLocationMaterialInventoryInfo(int placeid, int materialid, int roomid);

        /// <summary>
        /// 출고할 품목 LIST 반환 - FRONT용
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        Task<List<InOutInventoryDTO>?> AddOutStoreList(int placeid, int roomid, int materialid, int outcount);

        /// <summary>
        /// 대쉬보드용 품목별 재고현황
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<InventoryAmountDTO>?> GetInventoryAmountList(int placeid, List<int> MaterialIdx);

    }
}
