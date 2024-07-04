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
        /// 삭제할 사업장 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<PlaceTb?> GetDeletePlaceInfo(int? id);

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
        ValueTask<bool?> DeletePlace(PlaceTb? model);


    }
}
