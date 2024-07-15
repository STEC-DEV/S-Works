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
        /// 사업장에 속한 기계설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<MachineFacilityListDTO>?> GetPlaceMachineFacilityList(int? placeid);

        /// <summary>
        /// 공간ID에 포함되어있는 전체 설비LIST 조회
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        ValueTask<List<FacilityTb>?> GetAllFacilityList(int? roomid);

        /// <summary>
        /// 설비ID로 단일 설비모델 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<FacilityTb?> GetFacilityInfo(int? id);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateFacilityInfo(FacilityTb? model);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteFacilityInfo(FacilityTb? model);

    }
}
