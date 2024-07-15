using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Facility.ItemKey
{
    public interface IFacilityItemKeyInfoRepository
    {
        /// <summary>
        /// 그룹의 KEY 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<FacilityItemKeyTb?> AddAsync(FacilityItemKeyTb? model);

        /// <summary>
        /// 그룹ID에 포함되어있는 KEY 리스트 전체 반환
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        ValueTask<List<FacilityItemKeyTb>?> GetAllKeyList(int? groupitemid);

        /// <summary>
        /// KeyID에 해당하는 KEY모델 반환
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        ValueTask<FacilityItemKeyTb?> GetKeyInfo(int? keyid);

        /// <summary>
        /// Key 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateKeyInfo(FacilityItemKeyTb? model);

        /// <summary>
        /// Key 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteKeyInfo(FacilityItemKeyTb? model);
        
    }
}
