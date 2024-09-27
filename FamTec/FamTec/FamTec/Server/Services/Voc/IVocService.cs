using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocService
    {

        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<AllVocListDTO>> GetVocList(HttpContext context, List<int> type, List<int> status, List<int> buildingid);

        /// <summary>
        /// 조건별 민원 리스트 조회
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetVocFilterList(HttpContext context, DateTime startdate, DateTime enddate, List<int> type, List<int> status, List<int> buildingid);

        /// <summary>
        /// VOC 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocEmployeeDetailDTO>> GetVocDetail(HttpContext context, int vocid);

        /// <summary>
        /// VOC 유형 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateVocTypeService(HttpContext context, UpdateVocDTO dto);

        /// <summary>
        /// DashBoard용 일주일치 민원 각 타입별 카운트
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<VocWeekCountDTO>?> GetVocDashBoardDataService(HttpContext context);
    }
}
