using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group.Key.Value
{
    public class UpdateValueDTO
    {
        /// <summary>
        /// 아이템 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "수정하실 Value의 ID는 공백일 수 없습니다.")]
        public int ID { get; set; }

        /// <summary>
        /// 값
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "수정하실 값은 공백일 수 없습니다.")]
        public string ItemValue { get; set; } = null!;
    }
}
