using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;

namespace FamTec.Server.Repository.UseMaintenence
{
    public interface IUseMaintenenceInfoRepository
    {
        /// <summary>
        /// 사용자재 이력 TABLE 조회
        /// </summary>
        /// <param name="useid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<UseMaintenenceMaterialTb?> GetUseMaintanceInfo(int useid, int placeid);

        /// <summary>
        /// 사용자재 세부 이력 리스트
        /// </summary>
        /// <param name="useid"></param>
        /// <returns></returns>
        Task<UseMaterialDetailDTO?> GetDetailUseStoreList(int usematerialid, int placeid);

        /// <summary>
        /// 가용한 자재 수량 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        Task<int?> UseAvailableMaterialNum(int placeid, int roomid, int materialid);

        /// <summary>
        /// 해당건 사용자재테이블에서 사용된 품목의 출고개수 반환 
        ///     -- 입고냐 출고냐 정해야 함.
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="usemaintanceid"></param>
        /// <returns></returns>
        Task<int?> UseThisMaterialNum(int placeid, int maintenanceid, int roomid, int materialid);

        /// <summary>
        /// 가지고있는 개수가 요청 개수보다 많은지 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="delCount"></param>
        /// <returns></returns>
        Task<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delCount);

        /// <summary>
        /// 사용자재 수정 - 출고
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="updater"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int?> UseMaintanceOutput(int placeid, string updater, UpdateMaintenanceMaterialDTO dto);

        /// <summary>
        /// 사용자재 수정 - 입고
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="updater"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int?> UseMatintanceInput(int placeid, string updater, UpdateMaintenanceMaterialDTO dto);
    }
}
