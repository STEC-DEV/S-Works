using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;

namespace FamTec.Server.Services.Admin.Department
{
    public interface IDepartmentService
    {
        /// <summary>
        /// 부서추가 [수정완료]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<AddDepartmentDTO>> AddDepartmentService(HttpContext? context, AddDepartmentDTO? dto);

        /// <summary>
        /// 부서전체조회 [수정완료]
        /// </summary>
        /// <returns></returns>
        public ValueTask<ResponseList<DepartmentDTO>> GetAllDepartmentService();

        /// <summary>
        /// 부서삭제 [수정완료]
        /// </summary>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool>> DeleteDepartmentService(List<int?> index);

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<DepartmentDTO>?> UpdateDepartmentService(HttpContext? context, DepartmentDTO? dto);

    }
}
