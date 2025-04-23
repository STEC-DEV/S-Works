using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupInfoDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        [Required]
        public int BuildingIdx { get; set; }

        /// <summary>
        /// 그룹명칭
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;
    }
}
