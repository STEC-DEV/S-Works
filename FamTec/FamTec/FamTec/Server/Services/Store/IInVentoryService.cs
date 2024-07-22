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
        public ValueTask<ResponseUnit<AddInventoryDTO>?> AddInStoreService(HttpContext? context, AddInventoryDTO? dto);

        /// <summary>
        /// 입출고 이력
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<InOutHistoryListDTO>> GetInOutHistoryService(HttpContext? context);

        /// <summary>
        /// 삭제대상 갯수 구하기
        /// </summary>
        /// <param name="materialid"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> GetOutCountService(HttpContext? context, int? materialid, int? roomid);

        public ValueTask<ResponseList<bool?>> OutInventoryService(HttpContext? context, int? materialid, int? roomid);

     
    }
}


// 품목코드
// 품목이름
// 제조사
// 규격
