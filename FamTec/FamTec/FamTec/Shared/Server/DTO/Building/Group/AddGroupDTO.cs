using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupDTO
    {
        /// <summary>
        /// 건물인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 인덱스는 공백일 수 없습니다.")]
        public int BuildingIdx { get; set; }
        /// <summary>
        /// 명칭 ex) 주차장
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "그룹 이름은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        public List<AddGroupItemKeyDTO> AddGroupKey { get; set; } = new List<AddGroupItemKeyDTO>();

    }
}
