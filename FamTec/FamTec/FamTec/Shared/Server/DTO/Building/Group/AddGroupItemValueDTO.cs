using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupItemValueDTO
    {
        /// <summary>
        /// 값
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 값은 공백일 수 없습니다.")]
        public string Values { get; set; } = null!;
    }
}
