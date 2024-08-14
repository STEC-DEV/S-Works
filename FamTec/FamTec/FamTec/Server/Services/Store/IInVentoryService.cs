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
        public ValueTask<ResponseList<PeriodicInventoryRecordDTO>> PeriodicInventoryRecordService(HttpContext context, int materialid, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 사업장별 재고 현황
        /// </summary>
        /// <param name="context"></param>
        /// <param name="materialid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<MaterialHistory>> GetPlaceInventoryRecordService(HttpContext context, List<int> materialid, bool type);
    }
}