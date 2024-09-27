using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Unit;

namespace FamTec.Server.Repository.Unit
{
    public interface IUnitInfoRepository
    {
        /// <summary>
        /// 사업장별 단위정보 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<UnitTb>?> GetUnitList(int placeid);

        /// <summary>
        /// 해당사업장에 단위 추가되는지 여부
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<bool?> AddUnitInfoCheck(string unit, int placeid);

        /// <summary>
        /// 단위 인덱스로 단위정보 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        Task<UnitTb?> GetUnitInfo(int UnitIdx);

        /// <summary>
        /// 사업장별 단위정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UnitTb?> AddAsync(UnitTb model);

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteUnitInfo(List<int> idx, string deleter);

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateUnitInfo(UnitTb model);

    }
}
