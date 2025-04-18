using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// 민원 데이터 IMPORT
    /// </summary>
    public class ImportVocData
    {
        /// <summary>
        /// 민원의 제목
        /// </summary>
        [Required]
        public string title { get; set; } = null!;

        /// <summary>
        /// 민원의 내용
        /// </summary>
        [Required]
        public string contents { get; set; } = null!;

        /// <summary>
        /// 민원 작성자
        /// </summary>
        [Required]
        public string createUser { get; set; } = null!;

        /// <summary>
        /// 분류
        /// 0 : 미분류
        /// 1 : 기계
        /// 2 : 전기
        /// 3 : 승강
        /// 4 : 소방
        /// 5 : 건축
        /// 6 : 통신
        /// 7 : 미화
        /// 8 : 보안
        /// </summary>
        [Required]
        public string? type { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? phone { get; set; }
        
        /// <summary>
        /// 처리상태
        /// </summary>
        public string? status { get; set; }

        /// <summary>
        /// 민원 발생시간
        /// </summary>
        public DateTime? createDT { get; set; }

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
        /// 실패유무
        /// </summary>
        public bool failYn { get; set; } = false;
    }
}
