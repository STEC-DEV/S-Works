using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using FamTec.Server.Helpers;

namespace FamTec.Server.Services.Facility.Type.Contstruct
{
    public interface IConstructFacilityService
    {
        /// <summary>
        /// 건축설비 엑셀 양식 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<byte[]?> DownloadConstructFacilityForm();

        /// <summary>
        /// 건축설비 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool>> ImportConstructFacilityService(IFormFile? file);

        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<FacilityDTO>> AddConstructFacilityService(FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<FacilityListDTO>>> GetConstructFacilityListService();

        /// <summary>
        /// 설비ID의 설비 모델 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public Task<ResponseModel<FacilityDetailDTO>> GetConstructDetailFacilityService(int facilityId, bool isMobile);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateConstructFacilityService(FacilityDTO dto, IFormFile? files);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteConstructFacilityService(List<int> delIdx);
    }
}
