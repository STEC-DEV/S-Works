﻿namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 매니저 추가 DTO
    /// </summary>
    public class AddManagerDTO
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }


        /// <summary>
        /// 부서INDEX
        /// </summary>
        public int? DepartmentId { get; set; }

    }
}
