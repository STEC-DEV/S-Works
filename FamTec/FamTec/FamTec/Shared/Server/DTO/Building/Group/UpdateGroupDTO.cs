using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class UpdateGroupDTO
    {
        /// <summary>
        /// 그룹 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "그룹의 ID는 공백일 수 없습니다.")]
        public int GroupId { get; set; }

        /// <summary>
        /// 변경할 그룹명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "그룹의 이름은 공백일 수 없습니다.")]
        public string GroupName { get; set; } = null!;
    }
}
