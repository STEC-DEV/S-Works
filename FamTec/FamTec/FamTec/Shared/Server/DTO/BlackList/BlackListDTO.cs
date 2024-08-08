using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.BlackList
{
    /// <summary>
    /// 블랙리스트 조회 DTO
    /// </summary>
    public class BlackListDTO
    {
        /// <summary>
        /// 블랙리스트 테이블 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "블랙리스트 테이블 인덱스는 공백일 수 없습니다.")]
        public int ID { get; set; }

        /// <summary>
        /// 휴대폰 번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "휴대폰 번호는 공백일 수 없습니다.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
