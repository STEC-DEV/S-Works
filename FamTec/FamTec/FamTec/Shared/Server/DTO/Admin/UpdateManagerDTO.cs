﻿namespace FamTec.Shared.Server.DTO.Admin
{
    /// <summary>
    /// 관리자 수정 DTO
    /// </summary>
    public class UpdateManagerDTO
    {
        /// <summary>
        /// 관리자 테이블ID
        /// </summary>
        public int? AdminIndex { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 부서인덱스
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 로그인 ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 사업장
        /// </summary>
        public List<AdminPlaceDTO>? PlaceList { get; set; } = new List<AdminPlaceDTO>();

    }
}
