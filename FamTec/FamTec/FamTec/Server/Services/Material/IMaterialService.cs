using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public interface IMaterialService
    {
        /// <summary>
        /// 대쉬보드용 안전재고와 가까운 TOP10
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MaterialCountDTO>?> GetMaterialCountService(HttpContext context);

        /// <summary>
        /// 품목 엑셀 양식 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<byte[]?> DownloadMaterialForm(HttpContext context);

        /// <summary>
        /// 품목 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool>> ImportMaterialService(HttpContext context, IFormFile? file);

        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddMaterialDTO>> AddMaterialService(HttpContext context, AddMaterialDTO dto, IFormFile? files);

        /// <summary>
        /// 사업장의 전체 자재리스트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MaterialListDTO>> GetPlaceMaterialListService(HttpContext context, bool isMobile);
        
        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력 - Search용
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<MaterialSearchListDTO>> GetAllPlaecMaterialSearchService(HttpContext context);

        /// <summary>
        /// 사업장에 속해있는 자재리스트 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseUnit<int?>> GetPlaceMaterialCountService(HttpContext context);

       
        /// <summary>
        /// 사업장에 속해있는 자재 리스트 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Task<ResponseList<MaterialListDTO>> GetPlaceMaterialPageNationListService(HttpContext context, int pagenumber, int pagesize);

        /// <summary>
        /// 자재 상세정보 보기
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DetailMaterialDTO>> GetDetailMaterialService(HttpContext context,int materialid, bool isMobile);



        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateMaterialService(HttpContext context, UpdateMaterialDTO dto, IFormFile? files);

        /// <summary>
        /// 자재정보 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteMaterialService(HttpContext context, List<int> delIdx);

        /// <summary>
        /// 품목검색
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public Task<ResponseList<MaterialSearchListDTO>> GetMaterialSearchService(HttpContext context, string searchData);

        

    }
}
