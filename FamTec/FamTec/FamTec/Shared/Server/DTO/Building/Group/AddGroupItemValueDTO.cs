using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Building.Group
{
    public class AddGroupItemValueDTO
    {
        /// <summary>
        /// 값
        /// </summary>
        [Required]
        public string Values { get; set; } = null!;
    }
}
