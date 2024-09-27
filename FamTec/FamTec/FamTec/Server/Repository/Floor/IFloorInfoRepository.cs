using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Floor
{
    public interface IFloorInfoRepository
    {
        /// <summary>
        /// 층 삭제여부
        /// </summary>
        /// <param name="floortbid"></param>
        /// <returns></returns>
        Task<bool?> DelFloorCheck(int floortbid);

        /// <summary>
        /// 층추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<FloorTb?> AddAsync(FloorTb model);

        /// <summary>
        /// 건물에 해당하는 층리스트 조회
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        Task<List<FloorTb>?> GetFloorList(int buildingtbid);

        /// <summary>
        /// 층인덱스에 해당하는 모델 조회
        /// </summary>
        /// <param name="flooridx"></param>
        /// <returns></returns>
        Task<FloorTb?> GetFloorInfo(int flooridx);

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteFloorInfo(FloorTb model);

        /// <summary>
        /// 층 삭제
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        Task<bool?> DeleteFloorInfo(List<int> roomidx, string deleter);

        /// <summary>
        /// 층 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateFloorInfo(FloorTb model);

        /// <summary>
        /// 건물에 해당하는 층List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<FloorTb>?> GetFloorList(List<BuildingTb> model);
    }
}
