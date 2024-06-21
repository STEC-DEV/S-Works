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
        ValueTask<DepartmentTb?> AddAsync(DepartmentTb? model);

        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        ValueTask<List<DepartmentTb>?> GetAllList();

        /// <summary>
        /// 부서INDEX에 해당하는 부서 조회
        /// </summary>
        /// <param name="departmentidx"></param>
        /// <returns></returns>
        ValueTask<DepartmentTb?> GetDepartmentInfo(int? Id);

        /// <summary>
        /// 무서명에 해당하는 부서 조회
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        ValueTask<DepartmentTb?> GetDepartmentInfo(string? Name);


        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteDepartmentInfo(List<int?> selList);

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateDepartmentInfo(DepartmentTb? model);

    }
}
