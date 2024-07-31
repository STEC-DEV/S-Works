using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocService
    {
        /// <summary>
        /// 민원추가
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> AddVocService(AddVocDTO? dto, List<IFormFile>? image);

        /// <summary>
        /// Voc List 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<VocListDTO>?> GetVocList(HttpContext? context, DateTime? startdate, DateTime? enddate, int? type, int? status, int? buildingid);

        /// <summary>
        /// VOC 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<VocDetailDTO?>> GetVocDetail(HttpContext? context, int? vocid);

        /// <summary>
        /// VOC 유형 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateVocService(HttpContext? context, UpdateVocDTO? dto);
    }
}
