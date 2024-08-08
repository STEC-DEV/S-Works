using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group.Key.Value
{
    public class AddValueDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "KEY ID는 공백일 수 없습니다.")]
        public int KeyID { get; set; }

        /// <summary>
        /// 값 명칭
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "값은 공백일 수 없습니다.")]
        public string Value { get; set; } = null!;
    }
}
