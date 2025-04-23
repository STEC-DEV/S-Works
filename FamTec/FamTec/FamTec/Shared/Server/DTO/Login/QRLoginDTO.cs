using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Login
{
    public class QRLoginDTO
    {
        /// <summary>
        /// 사용자 로그인ID
        /// </summary>
        [Required]
        public string? UserId { get; set; } = null!;

        /// <summary>
        /// 사용자 로그인PW
        /// </summary>
        [Required]
        public string? UserPassword { get; set; } = null!;

        /// <summary>
        /// 사업장ID
        /// </summary>
        [Required]
        public int placeid { get; set; }
    }
}
