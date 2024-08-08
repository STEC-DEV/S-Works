using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.BlackList
{
    /// <summary>
    /// 블랙리스트 추가 DTO
    /// </summary>
    public class AddBlackListDTO
    {
        /// <summary>
        /// 휴대폰 번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 휴대폰 번호는 공백일 수 없습니다.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
