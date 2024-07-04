using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building
{
    public interface IBuildingInfoRepository
    {
        /// <summary>
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<BuildingTb?> AddAsync(BuildingTb? model);

        /// <summary>
        /// 해당사업장의 건물조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<BuildingTb>?> GetAllBuildingList(int? placeid);

        /// <summary>
        /// 빌딩인덱스로 빌뎅검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        ValueTask<BuildingTb?> GetBuildingInfo(int? buildingId);


        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateBuildingInfo(BuildingTb? model);

        ValueTask<List<BuildingTb>?> GetDeleteList(List<int>? buildingId);

        ValueTask<List<BuildingTb>?> GetBuildings(List<int>? buildingid);

        /// <summary>
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<BuildingTb>?> SelectPlaceBuildingList(List<int>? placeidx);

        ValueTask<bool?> DeleteBuildingInfo(BuildingTb? model);

    }
}
