using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Msign
{
    public class QRLoginDTO
    {
        /// <summary>
        /// 사용자 로그인ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 사용자 로그인PW
        /// </summary>
        public string? UserPassword { get; set; }

        /// <summary>
        /// 사업장ID
        /// </summary>
        public int placeid { get; set; } = 0;
    }
}
