using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Admin.Departmnet
{
    public interface IDepartmentInfoRepository
    {

        /// <summary>
        /// 부서 삭제가능 여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        Task<bool?> DelDepartmentCheck(int departmentid);

        /// <summary>
        /// 부서 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<DepartmentsTb?> AddAsync(DepartmentsTb model);

        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        Task<List<DepartmentsTb>?> GetAllList();

        /// <summary>
        /// 관리부서 조회
        /// </summary>
        /// <returns></returns>
        Task<List<DepartmentsTb>?> GetManageDepartmentList();

        /// <summary>
        /// 부서INDEX에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        Task<DepartmentsTb?> GetDepartmentInfo(int Id);

        /// <summary>
        /// 무서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<DepartmentsTb?> GetDepartmentInfo(string Name);

        /// <summary>
        /// 선택한 부서의 관리자 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        Task<List<AdminTb>?> SelectDepartmentAdminList(List<int> departmentidx);

        /// <summary>
        /// 삭제할 부서 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DepartmentsTb?> GetDeleteDepartmentInfo(int id);


        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteDepartment(DepartmentsTb model);

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        Task<bool?> DeleteDepartmentInfo(List<int> idx, string deleter);


        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateDepartmentInfo(DepartmentsTb model);

    }
}
