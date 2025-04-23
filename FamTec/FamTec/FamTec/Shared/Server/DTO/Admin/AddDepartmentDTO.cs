using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 부서 추가DTO
    /// </summary>
    public class AddDepartmentDTO
    {
        /// <summary>
        /// 부서 명
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 관리부서 여부
        /// </summary>
        public bool ManagerYN { get; set; } = false;
    }
}
