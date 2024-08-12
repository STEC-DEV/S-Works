using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;

namespace FamTec.Server.Services.Facility.Type.Electronic
{
    public interface IElectronicFacilityService
    {
        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<FacilityDTO>> AddElectronicFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<FacilityListDTO>> GetElectronicFacilityListService(HttpContext context);

        /// <summary>
        /// 설비ID의 설비 모델 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<FacilityDetailDTO>> GetElectronicDetailFacilityService(HttpContext context, int facilityId);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateElectronicFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteElectronicFacilityService(HttpContext context, List<int> delIdx);
    }
}
