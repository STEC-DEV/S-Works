using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;

namespace FamTec.Server.Services.Facility
{
    public interface IFacilityService
    {
        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddFacilityDTO>?> AddFacilityService(HttpContext? context, AddFacilityDTO? dto);

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<FacilityListDTO>?> GetFacilityListService(HttpContext? context);

        public ValueTask<ResponseUnit<FacilityDetailDTO>?> GetDetailService(int? facilityId);

    }
}
