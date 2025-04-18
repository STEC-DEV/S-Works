using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Voc
{
    public class ConvertVocData
    {
        /// <summary>
        /// Convert 민원의 제목
        /// </summary>
        [Required]
        public string title { get; set; } = null!;

        /// <summary>
        /// Convert 민원의 내용
        /// </summary>
        [Required]
        public string contents { get; set; } = null!;

        /// <summary>
        /// Convert 민원 작성자
        /// </summary>
        [Required]
        public string createUser { get; set; } = null!;

        /// <summary>
        /// Convert 분류
        /// </summary>
        [Required]
        public int type { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? phone { get; set; }

        /// <summary>
        /// Convert 처리상태
        /// </summary>
        [Required]
        public int status { get; set; }

        /// <summary>
        /// 민원 발생시간
        /// </summary>
        [Required]
        public DateTime createDT { get; set; }

        /// <summary>
        /// 완료시간
        /// </summary>
        public DateTime? completeDT { get; set; }

        /// <summary>
        /// 처리내용
        /// </summary>
        public string? completeContents { get; set; }

        /// <summary>
        /// 건물인덱스
        /// </summary>
        public int buildingIdx { get; set; }

        /// <summary>
        /// 사용자 인덱스
        /// </summary>
        public int userIdx { get; set; }

        /// <summary>
        /// 민원처리 담당자
        /// </summary>
        public string updateUser { get; set; }
    }
}
