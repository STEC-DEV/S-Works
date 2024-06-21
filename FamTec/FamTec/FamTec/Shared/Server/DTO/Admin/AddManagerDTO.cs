using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Admin
{
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
        /// 
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// 계정유형
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 부서INDEX
        /// </summary>
        public int? DepartmentId { get; set; }

       
    }
}
