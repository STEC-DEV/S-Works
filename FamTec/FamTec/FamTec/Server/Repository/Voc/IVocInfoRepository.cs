using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
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
        ValueTask<VocTb?> AddAsync(VocTb model);

        /// <summary>
        /// 사업장별 민원리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<AllVocListDTO>?> GetVocList(int placeid);

        /// <summary>
        /// 조건별 민원리스트 조회
        /// </summary>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        ValueTask<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, int Type, int status, int BuildingID);

        /// <summary>
        /// ID로 민원 상세조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        ValueTask<VocTb?> GetVocInfoById(int vocid);

        /// <summary>
        /// VOC 코드로 단일모델 조회
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ValueTask<VocTb?> GetVocInfoByCode(string code);


        ValueTask<bool> UpdateVocInfo(VocTb model);

      
    }
}
