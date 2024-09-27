using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;

namespace FamTec.Server.Services.Admin.Department
{
    public interface IDepartmentService
    {
        /// <summary>
        /// 부서추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<AddDepartmentDTO>> AddDepartmentService(HttpContext context, AddDepartmentDTO dto);

        /// <summary>
        /// 부서전체조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<DepartmentDTO>> GetAllDepartmentService();

        /// <summary>
        /// 관리부서 전체조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseList<DepartmentDTO>> ManageDepartmentService();

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> DeleteDepartmentService(HttpContext context, List<int> index);

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<DepartmentDTO>> UpdateDepartmentService(HttpContext context, DepartmentDTO dto);

    }
}
