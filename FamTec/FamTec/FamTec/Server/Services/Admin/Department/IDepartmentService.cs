using FamTec.Server.Helpers;
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
        public Task<ResponseModel<AddDepartmentDTO>> AddDepartmentService(AddDepartmentDTO dto);

        /// <summary>
        /// 부서전체조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<List<DepartmentDTO>>> GetAllDepartmentService();

        /// <summary>
        /// 관리부서 전체조회
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel<List<DepartmentDTO>>> ManageDepartmentService();

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> DeleteDepartmentService(List<int> index);

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseModel<DepartmentDTO>> UpdateDepartmentService(DepartmentDTO dto);

    }
}
