using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;

namespace FamTec.Server.Services.Facility.Type.Beauty
{
    public interface IBeautyFacilityService
    {
        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<FacilityDTO>> AddBeautyFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<FacilityListDTO>> GetBeautyFacilityListService(HttpContext context);

        /// <summary>
        /// 설비ID의 설비 모델 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<FacilityDetailDTO>> GetBeautyDetailFacilityService(HttpContext context, int facilityId);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateBeautyFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteBeautyFacilityService(HttpContext context, List<int> delIdx);
    }
}
