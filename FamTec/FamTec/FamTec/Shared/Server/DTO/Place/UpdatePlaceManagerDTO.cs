using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Place
{
    public class UpdatePlaceManagerDTO
    {
        /// <summary>
        /// 대상 사업장ID
        /// </summary>
        [Required]
        public int PlaceId { get; set; }

        /// <summary>
        /// 대상 Target AdminID
        /// </summary>
        public List<int> AdminId { get; set; } = new List<int>();
    }
}
