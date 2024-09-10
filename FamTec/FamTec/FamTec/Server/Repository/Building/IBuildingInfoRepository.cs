using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building
{
    public interface IBuildingInfoRepository
    {
        /// <summary>
        /// 삭제가능여부 체크
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        ValueTask<bool?> DelBuildingCheck(int buildingid);

        /// <summary>
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<BuildingTb?> AddAsync(BuildingTb model);

        /// <summary>
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <returns></returns>
        ValueTask<int?> TotalBuildingCount(int placeid);

        /// <summary>
        /// 해당사업장의 건물조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<BuildingTb>?> GetAllBuildingList(int placeid);

        /// <summary>
        /// 해당 사업장의 건물조회 - 페이지네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        ValueTask<List<BuildingTb>?> GetAllBuildingPageList(int placeid, int skip, int take);

        /// <summary>
        /// 빌딩인덱스로 빌딩검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        ValueTask<BuildingTb?> GetBuildingInfo(int buildingId);

        /// <summary>
        /// 사용가능한 건물코드인지 검사
        /// </summary>
        /// <param name="buildingcode"></param>
        /// <returns></returns>
        ValueTask<bool?> CheckBuildingCD(string buildingcode);

        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateBuildingInfo(BuildingTb model);

        ValueTask<List<BuildingTb>?> GetDeleteList(List<int> buildingId);

        ValueTask<List<BuildingTb>?> GetBuildings(List<int> buildingid);

        /// <summary>
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<BuildingTb>?> SelectPlaceBuildingList(List<int> placeidx);

        ValueTask<bool?> DeleteBuildingInfo(BuildingTb model);

        /// <summary>
        /// 건물정보 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteBuildingList(List<int> buildingid, string deleter);
        

    }
}
