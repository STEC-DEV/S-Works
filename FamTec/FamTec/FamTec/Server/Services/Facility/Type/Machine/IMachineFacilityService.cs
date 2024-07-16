using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;

namespace FamTec.Server.Services.Facility.Type.Machine
{
    public interface IMachineFacilityService
    {
        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddFacilityDTO>?> AddMachineFacilityService(HttpContext? context, AddFacilityDTO? dto, IFormFile? files);

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<FacilityListDTO>?> GetMachineFacilityListService(HttpContext? context);

        /// <summary>
        /// 설비ID의 설비 모델 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<FacilityDetailDTO?>> GetMachineDetailFacilityService(HttpContext? context, int? facilityId);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> UpdateMachineFacilityService(HttpContext? context, UpdateFacilityDTO? dto, IFormFile? files);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> DeleteMachineFacilityService(HttpContext? context, List<int> delIdx);

        // 사업장에 설치되어있는 기계설비 List 조회

    }
}
