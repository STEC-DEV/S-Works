using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Voc
{
    public interface IVocInfoRpeository
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
        ValueTask<List<ListVoc>?> GetVocList(List<BuildingTb>? buildinglist, string? date);
    }
}
