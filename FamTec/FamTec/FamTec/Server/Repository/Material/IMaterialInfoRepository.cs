using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Material
{
    public interface IMaterialInfoRepository
    {
        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaterialTb?> AddAsync(MaterialTb? model);

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<List<MaterialTb>?> GetPlaceAllMaterialList(int? placeid);

        /// <summary>
        /// 건물에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public ValueTask<List<MaterialTb>?> GetBuildingAllMatertialList(int? buildingid);

        /// <summary>
        /// 자재 인덱스에 해당하는 모델클래스 반환
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public ValueTask<MaterialTb?> GetMaterialInfo(int? materialId);


    }
}
