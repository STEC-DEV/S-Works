using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Admin.Departmnet
{
    public interface IDepartmentInfoRepository
    {
        /// <summary>
        /// 부서 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<DepartmentsTb?> AddAsync(DepartmentsTb model);

        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<DepartmentsTb>?> GetAllList();

        /// <summary>
        /// 관리부서 조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<DepartmentsTb>?> GetManageDepartmentList();

        /// <summary>
        /// 부서INDEX에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        ValueTask<DepartmentsTb?> GetDepartmentInfo(int Id);

        /// <summary>
        /// 무서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        ValueTask<DepartmentsTb?> GetDepartmentInfo(string Name);

        /// <summary>
        /// 선택한 부서의 관리자 반환
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        ValueTask<List<AdminTb>?> SelectDepartmentAdminList(List<int> departmentidx);

        /// <summary>
        /// 삭제할 부서 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<DepartmentsTb?> GetDeleteDepartmentInfo(int id);


        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteDepartment(DepartmentsTb model);

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteDepartmentInfo(List<int> idx, string deleter);


        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateDepartmentInfo(DepartmentsTb model);

    }
}
