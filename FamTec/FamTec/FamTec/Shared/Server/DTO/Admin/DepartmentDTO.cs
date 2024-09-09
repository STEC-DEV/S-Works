namespace FamTec.Shared.Server.DTO.Admin
{
    public class DepartmentDTO
    {
        /// <summary>
        /// 부서 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        public bool IsSelect { get; set; } = false;

        /// <summary>
        /// 부서명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 관리부서 여부
        /// </summary>
        public bool? ManagerYN { get; set; }

        public string? Description { get; set; } = null;

    }
}
