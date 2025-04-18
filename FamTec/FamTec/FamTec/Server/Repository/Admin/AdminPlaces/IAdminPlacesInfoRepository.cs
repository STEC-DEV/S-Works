using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;

namespace FamTec.Server.Repository.Admin.AdminPlaces
{
    public interface IAdminPlacesInfoRepository
    {
        /// <summary>
        /// 관리자 사업장 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> AddAsync(List<AdminPlaceTb> model);

        /// <summary>
        /// 해당 사업장에서 관리자 삭제
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<bool?> RemoveAdminPlace(List<int> adminid, int placeid);

        /// <summary>
        /// 관리자 사업장 단일추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<AdminPlaceTb?> AddAdminPlaceInfo(AdminPlaceTb model);

        /// <summary>
        /// 관리자 사업장 단일 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteAdminPlaceInfo(AdminPlaceTb model);

        /// <summary>
        /// 관리자 사업장 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        Task<List<AdminPlaceDTO>?> GetMyWorks(int adminid);

        /// <summary>
        /// 관리자에 해당하는 사업장리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        Task<List<AdminPlaceTb>?> GetMyWorksList(int adminid);

        /// <summary>
        /// 관리자 로그인용 사업장 리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        Task<List<AdminPlaceDTO>?> LoginSelectPlaceList(int adminid);

        /// <summary>
        /// 관리자 상세보기 페이지
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        Task<DManagerDTO?> GetManagerDetails(int adminidx);

        /// <summary>
        /// AdminPlaceTb 사업장 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteMyWorks(AdminPlaceTb model);


        /// <summary>
        /// 사업장 정보 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<PlaceDetailDTO?> GetWorksInfo(int placeid);

        /// <summary>
        /// 관리자 사업장 조회 - 사업장 INDEX
        /// </summary>
        /// <param name="placetb"></param>
        /// <returns></returns>
        Task<AdminPlaceTb?> GetWorksModelInfo(int placeid);

        /// <summary>
        /// 관리자테이블 ID + 사업장ID에 해당하는 AdminPlaceTB 모델 조회
        /// </summary>
        /// <param name="admintbid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<AdminPlaceTb?> GetPlaceAdminInfo(int admintbid, int placeid);

        /// <summary>
        /// 해당 사업장에 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteAdminPlaceManager(AdminPlaceTb model);

        /// <summary>
        /// 선택된 사업장에 포함되어있는 AdminPlaceTB 리스트 반환 
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<AdminPlaceTb>?> SelectPlaceAdminList(List<int> placeidx);

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<(List<int>? insert, List<int>? delete)?> DisassembleUpdateAdminInfo(int adminid, List<int> placeidx);

        /// <summary>
        /// 관리자가 포함안된 사업장 리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        Task<List<AdminPlaceDTO>?> GetNotContainsPlaceList(int adminid);


        Task<int?> UpdatePlaceManager(UpdatePlaceManagerDTO dto, string updater);
    }
}
