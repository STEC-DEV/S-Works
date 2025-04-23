using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class UpdateValueDTO
    {
        /// <summary>
        /// 아이템 ID
        /// </summary>
        [Required]
        public int? ID { get; set; }

        /// <summary>
        /// 값
        /// </summary>
        public string? ItemValue { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }
    }
}
