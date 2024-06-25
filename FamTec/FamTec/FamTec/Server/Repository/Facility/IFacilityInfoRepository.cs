using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility;

namespace FamTec.Server.Repository.Facility
{
    public interface IFacilityInfoRepository
    {
        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<FacilityTb?> AddAsync(FacilityTb? model);

        /// <summary>
        /// 해당 사업장의 모든 설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<FacilityListDTO>?> GetFacilityList(int? placeid);

        ValueTask<FacilityDetailDTO?> GetDetailInfo(int? facilityId);
    }
}
