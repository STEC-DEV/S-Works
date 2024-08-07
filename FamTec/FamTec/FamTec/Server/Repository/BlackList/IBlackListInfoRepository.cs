using FamTec.Shared.Model;

namespace FamTec.Server.Repository.BlackList
{
    public interface IBlackListInfoRepository
    {
        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        ValueTask<BlacklistTb?> AddAsync(BlacklistTb? model);

        /// <summary>
        /// 블랙리스트 휴대폰번호로 조회
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        ValueTask<BlacklistTb?> GetBlackListInfo(string? PhoneNumber);

        /// <summary>
        /// 블랙리스트 인덱스로 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<BlacklistTb?> GetBlackListInfo(int? id);

        /// <summary>
        /// 블랙리스트 전체조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<BlacklistTb>?> GetBlackList();

        /// <summary>
        /// 블랙리스트 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateBlackList(BlacklistTb model);

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteBlackList(List<int>? delIdx, string? deleter);
    }
}
