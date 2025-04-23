using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupDTO
    {
        /// <summary>
        /// 건물인덱스
        /// </summary>
        [Required]
        public int BuildingIdx { get; set; }

        /// <summary>
        /// 명칭 ex) 주차장
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        public List<AddGroupItemKeyDTO>? AddGroupKey { get; set; } = new List<AddGroupItemKeyDTO>();

    }
}
