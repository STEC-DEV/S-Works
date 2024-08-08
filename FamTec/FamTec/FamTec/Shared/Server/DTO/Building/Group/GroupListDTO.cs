using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class GroupListDTO
    {
        /// <summary>
        /// 그룹 ListID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "그룹의 ID는 공백일 수 없습니다.")]
        public int ID { get; set; }

        /// <summary>
        /// 그룹명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "그룹 명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;


        public List<GroupKeyListDTO>? KeyListDTO { get; set; } = new List<GroupKeyListDTO>();
    }
}
