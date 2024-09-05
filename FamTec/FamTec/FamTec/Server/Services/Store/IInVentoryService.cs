using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Store
{
    public interface IInVentoryService
    {
        /// <summary>
        /// 입고
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> AddInStoreService(HttpContext context, List<InOutInventoryDTO> dto);

        /// <summary>
        /// 입출고 이력
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<InOutHistoryListDTO>> GetInOutHistoryService(HttpContext context);

        /// <summary>
        /// 사업장의 입-출고 이력 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> GetPlaceInOutCountService(HttpContext context);

        /// <summary>
        /// 입-출고 이력 페이지네이션 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<InOutHistoryListDTO>> GetInoutPageNationHistoryService(HttpContext context, int pagenum, int pagesize);

        /// <summary>
        /// 출고 등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<bool?>> OutInventoryService(HttpContext context, List<InOutInventoryDTO> dto);

        /// <summary>
        /// 품목별 기간별 입출고내역
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<PeriodicDTO>> PeriodicInventoryRecordService(HttpContext context, List<int> materialid, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 사업장별 재고 현황
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaterialHistory>> GetPlaceInventoryRecordService(HttpContext context, List<int> materialid, bool type);

        /// <summary>
        /// 해당 품목의 재고수량 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<InOutLocationDTO>> GetMaterialRoomNumService(HttpContext context, int MaterialId, int buildingid);

        /// <summary>
        /// 출고할 품목 LIST 반환 - FRONT용
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<InOutInventoryDTO>> AddOutStoreList(HttpContext context, int roomid, int materialid, int outcount);
    }
}