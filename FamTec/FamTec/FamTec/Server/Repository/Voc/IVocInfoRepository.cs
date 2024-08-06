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
        ValueTask<VocTb?> AddAsync(VocTb? model);

        /// <summary>
        /// 해당 월의 민원현황 조회
        /// </summary>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        ValueTask<List<VocListDTO>?> GetVocList(int? placeid, DateTime? StartDate, DateTime? EndDate, int? Type, int? status, int? BuildingID);

        /// <summary>
        /// 민원 상세조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        ValueTask<VocTb?> GetDetailVoc(int? vocid);

        /// <summary>
        /// VOC 코드로 단일모델 조회
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ValueTask<VocTb?> GetVocInfo(string? code);


        ValueTask<bool> UpdateVocInfo(VocTb? model);

        /// <summary>
        /// 해당일자의 VOC 건수 구하기
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        ValueTask<int?> VocDaysCount(DateTime now);
    }
}
