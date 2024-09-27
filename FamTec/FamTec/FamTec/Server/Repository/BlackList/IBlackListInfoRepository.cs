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
        Task<BlacklistTb?> AddAsync(BlacklistTb model);

        /// <summary>
        /// 블랙리스트 휴대폰번호로 조회
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        Task<BlacklistTb?> GetBlackListInfo(string PhoneNumber);

        /// <summary>
        /// 블랙리스트 인덱스로 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BlacklistTb?> GetBlackListInfo(int id);

        /// <summary>
        /// 블랙리스트 전체조회
        /// </summary>
        /// <returns></returns>
        Task<List<BlacklistTb>?> GetBlackList();

        /// <summary>
        /// 블랙리스트 페이지네이션 조회
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<List<BlacklistTb>?> GetBlackListPaceNationList(int pagenumber, int pagesize);

        /// <summary>
        /// 블랙리스트 개수 조회
        /// </summary>
        /// <returns></returns>
        Task<int> GetBlackListCount();

        /// <summary>
        /// 블랙리스트 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateBlackList(BlacklistTb model);

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        Task<bool?> DeleteBlackList(List<int> delIdx, string deleter);
    }
}
