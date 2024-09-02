using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Repository.Material
{
    public interface IMaterialInfoRepository
    {
        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaterialTb?> AddAsync(MaterialTb model);

        /// <summary>
        /// 엑셀 IMPORT 추가
        /// </summary>
        /// <param name="MaterialList"></param>
        /// <returns></returns>
        public ValueTask<bool?> AddMaterialList(List<MaterialTb> MaterialList);

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<List<MaterialTb>?> GetPlaceAllMaterialList(int placeid);

        

        /// <summary>
        /// 해당 품목코드 사업장에 있는지 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ValueTask<bool?> GetPlaceMaterialCheck(int placeid, string code);

        /// <summary>
        /// 사업장에 속해있는 자재 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<int> GetPlaceAllMaterialCount(int placeid);

        /// <summary>
        /// 사업장에 속해있는 자재 리스트 페이지네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ValueTask<List<MaterialTb>?> GetPlaceAllMaterialPageNationList(int placeid, int pagenumber, int pagesize);

        /// <summary>
        /// 자재 인덱스에 해당하는 모델클래스 반환
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public ValueTask<MaterialTb?> GetDetailMaterialInfo(int placeid, int materialId);

        /// <summary>
        /// 자재 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<bool?> UpdateMaterialInfo(MaterialTb model);

        /// <summary>
        /// 자재 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<bool?> DeleteMaterialInfo(List<int> delidx, string deleter);


        /// <summary>
        /// 품목 검색
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public ValueTask<List<MaterialSearchListDTO>> GetMaterialSearchInfo(int placeid, string searchData);
    }
}
