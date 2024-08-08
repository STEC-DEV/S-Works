using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building
{
    public class GroupValueListDTO
    {
        /// <summary>
        /// 아이템 ID
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 값
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "값은 공백일 수 없습니다.")]
        public string ItemValue { get; set; } = null!;

    }
}
