using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group.Key
{
    public class AddKeyDTO
    {
        [NotNull]
        [Required(ErrorMessage = "그룹ID는 공백일 수 없습니다.")]
        public int GroupID { get; set; } // 상위 그룹ID
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        public string? Name { get; set; } // Key 내용

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();
    }
}
