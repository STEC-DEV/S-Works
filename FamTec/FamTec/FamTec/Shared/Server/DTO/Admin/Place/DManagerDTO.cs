using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class DManagerDTO
    {
        /// <summary>
        /// 부서인덱스
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 관리자ID
        /// </summary>
        public int AdminId { get; set; }

        /// <summary>
        /// 관리자 이름
        /// </summary>
        [Display(Name = "이름")]
        public string? Name { get; set; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        [Display(Name = "아이디")]
        public string? UserId { get; set; }

        /// <summary>
        /// 관리자 비밀번호
        /// </summary>
        [Display(Name = "비밀번호")]
        public string? Password { get; set; }

        /// <summary>
        /// 관리자 이메일
        /// </summary>
        [Display(Name = "이메일")]
        public string? Email { get; set; }

        /// <summary>
        /// 관리자 전화번호
        /// </summary>
        [Display(Name = "전화번호")]
        public string? Phone { get; set; }

        /// <summary>
        /// 관리자 계정타입
        /// </summary>
        [Display(Name = "계정 유형")]
        public string? Type { get; set; }

        /// <summary>
        /// 관리자 부서명
        /// </summary>
        [Display(Name = "부서")]
        public string? Department { get; set; }

        /// <summary>
        /// 이미지 명칭
        /// </summary>
        public string? ImageName { get; set; }

        /// <summary>
        /// 관리자 이미지
        /// </summary>
        public byte[]? Image { get; set; }
    }
}
