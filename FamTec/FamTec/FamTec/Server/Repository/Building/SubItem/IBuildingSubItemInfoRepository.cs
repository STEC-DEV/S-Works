using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem
{
    public interface IBuildingSubItemInfoRepository
    {
        /// <summary>
        /// 건물 추가정보 등록
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<BuildingSubitemTb?> AddAsync(BuildingSubitemTb? model);

        /// <summary>
        /// 건물ID에 등록되어있는 추가항목 데이터들 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        ValueTask<List<BuildingSubitemTb>?> GetAllBuildingSubItemList(int? buildingId);

        /// <summary>
        /// SubId에 해당하는 모델 반환
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        ValueTask<BuildingSubitemTb?> GetBuildingSubItemInfo(int? subId);

        /// <summary>
        /// 건물 추가항목 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateBuildingSubItemInfo(BuildingSubitemTb? model);

        /// <summary>
        /// 건물ID에 해당하는 추가항목들 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteBuildingSubItemInfo(int? buildingid, string? username);
    }
}
