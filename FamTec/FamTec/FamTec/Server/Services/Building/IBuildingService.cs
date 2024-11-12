using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;

namespace FamTec.Server.Services.Building
{
    public interface IBuildingService
    {
        /// <summary>
        /// 건물 엑셀양식 다운로드
        /// </summary>
        /// <returns></returns>
        public Task<byte[]?> DownloadBuildingForm(HttpContext context);

        /// <summary>
        /// 건물 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> ImportBuildingService(HttpContext context, IFormFile files);

        /// <summary>
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseUnit<int?>> TotalBuildingCount(HttpContext context);

        /// <summary>
        /// 건물추가
        /// </summary>
        /// <returns></returns>
        public Task<ResponseUnit<AddBuildingDTO>> AddBuildingService(HttpContext context, AddBuildingDTO dto, IFormFile? files);

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물리스트 조회
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public Task<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext context);

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물리스트 조회 - 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public Task<ResponseList<BuildinglistDTO>> GetBuildingListPageService(HttpContext context, int skip, int take);

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물명 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<PlaceBuildingNameDTO>> GetPlaceBuildingNameService(HttpContext context);

        /// <summary>
        /// 사업장에 속한 건물-층 리스트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<PlaceBuildingListDTO>> GetPlaceBuildingService(HttpContext context);

        /// <summary>
        /// 건물 상세 정보 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DetailBuildingDTO>> GetDetailBuildingService(HttpContext context, int buildingId, bool isMobile);

        /// <summary>
        /// 건물 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateBuildingService(HttpContext context, DetailBuildingDTO dto, IFormFile? files);

        /// <summary>
        /// 건물 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteBuildingService(HttpContext context, List<int> buildingid);

        /// <summary>
        /// 건물 ID로 이름반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<string?>> GetBuildingName(int buildingid);

        /// <summary>
        /// 자재가 포함되어있는 건물 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public Task<ResponseList<PlaceBuildingNameDTO>> GetPlaceAvailableBuildingList(HttpContext context, int materialid);
    }
}
