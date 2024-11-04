namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// VOC 월별 조회
    /// </summary>
    public class AllVocListDTO
    {
        public int? Years { get; set; }
        public int? Month { get; set; }
        public string? Dates { get; set; }

        public List<VocListDTO> VocList { get; set; } = new List<VocListDTO>();
    }

    public class VocListDTO
    {
        /// <summary>
        /// VOC ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 유형
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 모바일 : 웹 구분
        /// </summary>
        public int? Division { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 처리상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 민원처리 완료 시간
        /// </summary>
        public string? CompleteDT { get; set; }

        /// <summary>
        /// 민원처리 소요 시간
        /// </summary>
        public string? DurationDT { get; set; }

        /// <summary>
        /// 요청일시
        /// </summary>
        public string? CreateDT { get; set; }

        /// <summary>
        /// 작성자
        /// </summary>
        public string? CreateUser { get; set; }
    }
}
