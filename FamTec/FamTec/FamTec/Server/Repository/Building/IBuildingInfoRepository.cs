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
        Task<bool?> DelBuildingCheck(int buildingid);

        /// <summary>
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BuildingTb?> AddAsync(BuildingTb model);

        /// <summary>
        /// 건물 여러개 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddBuildingList(List<BuildingTb> model);

        /// <summary>
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <returns></returns>
        Task<int?> TotalBuildingCount(int placeid);

        /// <summary>
        /// 해당사업장의 건물조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<BuildingTb>?> GetAllBuildingList(int placeid);

        /// <summary>
        /// 공간이 있는 건물List 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<BuildingTb>?> GetPlaceAvailableBuildingList(int placeid, int materialid);

        /// <summary>
        /// 해당 사업장의 건물조회 - 페이지네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<List<BuildingTb>?> GetAllBuildingPageList(int placeid, int skip, int take);

        /// <summary>
        /// 빌딩인덱스로 빌딩검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        Task<BuildingTb?> GetBuildingInfo(int buildingId);

        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateBuildingInfo(BuildingTb model);

        Task<List<BuildingTb>?> GetDeleteList(List<int> buildingId);

        Task<List<BuildingTb>?> GetBuildings(List<int> buildingid);

        /// <summary>
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<BuildingTb>?> SelectPlaceBuildingList(List<int> placeidx);

        Task<bool?> DeleteBuildingInfo(BuildingTb model);

        /// <summary>
        /// 건물정보 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> DeleteBuildingList(List<int> buildingid, string deleter);
        

    }
}
