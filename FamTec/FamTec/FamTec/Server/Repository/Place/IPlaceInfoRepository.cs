using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Place
{
    public interface IPlaceInfoRepository
    {
        /// <summary>
        /// 사업장 추가
        /// </summary>
        /// <returns></returns>
        Task<PlaceTb?> AddPlaceInfo(PlaceTb model);

        /// <summary>
        /// 건물ID로 사업장 정보 조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        Task<PlaceTb?> GetBuildingPlace(int buildingid);

        /// <summary>
        /// 사업장코드로 사업장 검색
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        Task<bool?> PlaceUKCheck(string ContractNum);


        /// <summary>
        /// 전제조회
        /// </summary>
        /// <returns></returns>
        Task<List<PlaceTb>?> GetAllList(); // 사용

        /// <summary>
        /// 사업장인덱스로 사업장 정보 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PlaceTb?> GetByPlaceInfo(int id);

        /// <summary>
        /// 삭제할 사업장 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PlaceTb?> GetDeletePlaceInfo(int id);

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> EditPlaceInfo(PlaceTb model); // 사용


        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeletePlace(PlaceTb model);

        Task<bool?> DeletePlaceList(string Name, List<int> placeidx);

    }
}
