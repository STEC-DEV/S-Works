using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Unit
{
    public interface IUnitInfoRepository
    {
        /// <summary>
        /// 사업장별 단위정보 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<UnitTb>?> GetUnitList(int? placeid);

        /// <summary>
        /// 단위 인덱스로 단위정보 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        ValueTask<UnitTb?> GetUnitInfo(int? UnitIdx);

        /// <summary>
        /// 사업장별 단위정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<UnitTb?> AddAsync(UnitTb? model);

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteUnitInfo(UnitTb? model);

        

    }
}
