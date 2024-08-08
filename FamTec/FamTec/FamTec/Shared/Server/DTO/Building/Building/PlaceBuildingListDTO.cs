using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Building.Building
{
    public class PlaceBuildingListDTO
    {
        /// <summary>
        /// 건물 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물 인덱스는 공백일 수 없습니다.")]
        public int BuildingId { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "건물명은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 층 리스트
        /// </summary>
        public List<BuildingFloor> FloorList { get; set; } = new List<BuildingFloor>();
    }


    public class BuildingFloor
    {
        /// <summary>
        /// 층 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "층 인덱스는 공백일 수 없습니다.")]
        public int FloorId { get; set; }

        /// <summary>
        /// 층 명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "층 이름은 공백일 수 없습니다.")]
        public string Name { get; set; } = null!;
    }

}
