using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// 민원등록 - 민원인 전용
    /// </summary>
    public class AddVocDTOV2
    {
        /// <summary>
        /// 모바일 여부 0 : 모바일 / 1: 웹
        /// </summary>
        public int division { get; set; }


        /// <summary>
        /// 미분류 - 기계, 전기 등...
        /// </summary>
        [Required]
        public int type { get; set; }

        /// <summary>
        /// 민원의 제목
        /// </summary>
        [Required]
        public string? title { get; set; }

        /// <summary>
        /// 민원의 내용
        /// </summary>
        [Required]
        public string? contents { get; set; }

        /// <summary>
        /// 작성자 이름
        /// </summary>
        [Required]
        public string? name { get; set; }

        /// <summary>
        /// 휴대전화 번호
        /// </summary>
        public string? phoneNumber { get; set; }

        /// <summary>
        /// 건물 ID
        /// </summary>
        [Required]
        public int buildingId { get; set; }

        /// <summary>
        /// 사업장 ID
        /// </summary>
        [Required]
        public int placeId { get; set; }

        /// <summary>
        /// 알림톡 발송할건지 여부
        /// Default - false
        /// </summary>
        public bool sendYn { get; set; } = false; 
    }
}
