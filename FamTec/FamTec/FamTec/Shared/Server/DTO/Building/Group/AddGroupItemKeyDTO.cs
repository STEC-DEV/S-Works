using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupItemKeyDTO
    {
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 Key의 이름은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 단위
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 단위 명칭은 공백일 수 없습니다")]
        public string Unit { get; set; } = null!;

        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();

    }
}
