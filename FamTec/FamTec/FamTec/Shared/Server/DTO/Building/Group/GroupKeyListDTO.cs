using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class GroupKeyListDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "Key의 ID값은 공백일 수 없습니다.")]
        public int ID { get; set; }

        /// <summary>
        /// 키의 명칭
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "Key의 이름은 공백일 수 없습니다.")]
        public string ItemKey { get; set; } = null!;

        /// <summary>
        /// 단위
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "단위명은 공백일 수 없습니다.")]
        public string Unit { get; set; } = null!;

        public List<GroupValueListDTO>? ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
