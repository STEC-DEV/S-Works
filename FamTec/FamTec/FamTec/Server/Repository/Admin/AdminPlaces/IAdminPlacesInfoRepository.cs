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
        ValueTask<bool?> AddAsync(List<AdminPlaceTb>? model);


        /// <summary>
        /// 관리자 사업장 단일추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AdminPlaceTb?> AddAdminPlaceInfo(AdminPlaceTb? model);

        /// <summary>
        /// 관리자 사업장 단일 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteAdminPlaceInfo(AdminPlaceTb? model);

        /// <summary>
        /// 관리자 사업장 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<List<AdminPlaceDTO>?> GetMyWorks(int? adminid);

        /// <summary>
        /// 관리자에 해당하는 사업장리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<List<AdminPlaceTb>?> GetMyWorksList(int? adminid);

        /// <summary>
        /// 관리자 상세보기 페이지
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        ValueTask<DManagerDTO?> GetManagerDetails(int? adminidx);

        /// <summary>
        /// 관리자사업장 리스트 모델에 해당하는 사업장 리스트들 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<List<PlaceTb>?> GetMyWorksDetails(List<AdminPlaceTb>? model);

        /// <summary>
        /// AdminPlaceTb 사업장 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteMyWorks(AdminPlaceTb? model);

        /// <summary>
        /// 사업장 정보 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<PlaceDetailDTO?> GetWorksInfo(int? placeid);

        /// <summary>
        /// 관리자 사업장 조회 - 사업장 INDEX
        /// </summary>
        /// <param name="placetb"></param>
        /// <returns></returns>
        ValueTask<AdminPlaceTb?> GetWorksModelInfo(int? placeid);

        /// <summary>
        /// 관리자테이블 ID + 사업장ID에 해당하는 AdminPlaceTB 모델 조회
        /// </summary>
        /// <param name="admintbid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<AdminPlaceTb?> GetPlaceAdminInfo(int? admintbid, int? placeid);

        /// <summary>
        /// 해당 사업장에 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteAdminPlaceManager(AdminPlaceTb? model);

        /// <summary>
        /// 선택된 사업장에 포함되어있는 AdminPlaceTB 리스트 반환 
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<AdminPlaceTb>?> SelectPlaceAdminList(List<int>? placeidx);

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<(List<int?>? insert, List<int?>? delete)?> DisassembleUpdateAdminInfo(int? adminid, List<int?> placeidx);

        /// <summary>
        /// 관리자가 포함안된 사업장 리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<List<AdminPlaceDTO?>> GetNotContainsPlaceList(int? adminid);
    }
}
