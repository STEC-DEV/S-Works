using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group.Key
{
    public class UpdateKeyDTO
    {
        /// <summary>
        /// 키 ID
        /// </summary>
        public int? ID { get; set; }

        [NotNull]
        [Required(ErrorMessage = "수정하실 그룹의 KEY 이름은 공백일 수 없습니다.")]
        public string Itemkey { get; set; } = null!;

        [NotNull]
        [Required(ErrorMessage = "수정하실 그룹의 KEY 단위명은 공백일 수 없습니다.")]
        public string Unit { get; set; } = null!;

        public List<GroupValueListDTO> ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
