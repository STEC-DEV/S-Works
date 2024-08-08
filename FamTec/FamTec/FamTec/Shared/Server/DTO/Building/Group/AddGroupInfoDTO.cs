using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupInfoDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 인덱스는 공백일 수 없습니다.")]
        public int BuildingIdx { get; set; }

        /// <summary>
        /// 그룹명칭
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "추가하실 그룹 이름은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;
    }
}
