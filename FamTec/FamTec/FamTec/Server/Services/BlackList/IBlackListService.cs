using FamTec.Server.Helpers;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.BlackList;

namespace FamTec.Server.Services.BlackList
{
    public interface IBlackListService
    {
        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<AddBlackListDTO>> AddBlackList(AddBlackListDTO dto);

        /// <summary>
        /// 블랙리스트 전체조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<List<BlackListDTO>>> GetAllBlackList();

        /// <summary>
        /// 블랙리스트 개수 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<int?>> GetBlackListCountService();

        /// <summary>
        /// 블랙리스트 페이지네이션 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<BlackListDTO>>> GetAllBlackListPageNation(int pagenumber, int pagesize);

        /// <summary>
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> UpdateBlackList(BlackListDTO dto);

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteBlackList(List<int> delIdx);

    }
}
