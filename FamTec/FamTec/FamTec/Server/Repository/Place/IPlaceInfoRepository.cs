using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Place
{
    public interface IPlaceInfoRepository
    {
        /// <summary>
        /// 사업장 추가
        /// </summary>
        /// <returns></returns>
        ValueTask<PlaceTb?> AddPlaceInfo(PlaceTb model);


        ValueTask<bool?> DeletePlaceList(string? Name, List<int>? placeidx);

        /// <summary>
        /// 전제조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<PlaceTb>?> GetAllList(); // 사용

        /// <summary>
        /// 사업장인덱스로 사업장 정보 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<PlaceTb?> GetByPlaceInfo(int? id);

        /// <summary>
        /// 사업장코드로 사업장 정보 조회
        /// </summary>
        /// <param name="placecd"></param>
        /// <returns></returns>
        ValueTask<PlaceTb?> GetByPlaceInfo(string? placecd);

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> EditPlaceInfo(PlaceTb? model); // 사용


        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeletePlaceInfo(PlaceTb? model);


    }
}
