using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Alarm
{
    /// <summary>
    /// 미열람 알람 DTO
    /// </summary>
    public class AlarmDTO
    {
        /// <summary>
        /// 알람ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "알람 인덱스는 공백일 수 없습니다.")]
        public int AlarmID { get; set; }

        /// <summary>
        /// VOC 제목
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "민원 제목은 공백일 수 없습니다.")]
        public string VocTitle { get; set; } = null!;
    }
}
