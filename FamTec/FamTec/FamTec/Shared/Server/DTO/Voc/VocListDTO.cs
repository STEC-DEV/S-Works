namespace FamTec.Shared.Server.DTO.Voc
{
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
    }
}
