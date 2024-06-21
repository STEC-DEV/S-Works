using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Login
{
    public class LoginDTO
    {
        /// <summary>
        /// 로그인 ID
        /// </summary>
        public string? UserID { get; set; }

        /// <summary>
        /// 로그인 PASSWORD
        /// </summary>
        public string? UserPassword { get; set; }
    }
}
