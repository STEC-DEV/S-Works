using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Repository.Voc
{
    public interface IVocInfoRepository
    {
        /// <summary>
        /// VOC 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<VocTb?> AddAsync(VocTb model);

        /// <summary>
        /// 사업장별 민원리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<AllVocListDTO>?> GetVocList(int placeid, List<int> type, List<int> status, List<int> buildingid);

        /// <summary>
        /// 조건별 민원리스트 조회
        /// </summary>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status, List<int> BuildingID);

        /// <summary>
        /// ID로 민원 상세조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        Task<VocTb?> GetVocInfoById(int vocid);

        /// <summary>
        /// VOC 코드로 단일모델 조회
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<VocTb?> GetVocInfoByCode(string code);


        Task<bool> UpdateVocInfo(VocTb model);

        /// <summary>
        /// DashBoard 용 일주일치 각 타입별 카운트
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        Task<List<VocWeekCountDTO>?> GetDashBoardData(DateTime StartDate, DateTime EndDate);
      
    }
}
