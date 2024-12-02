namespace FamTec.Shared.Server.DTO.Material
{
    public class MaterialWeekCountDTO
    {
        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 입출고 이력
        /// </summary>
        public List<WeekInOutCountDTO>? WeekCountList { get; set; }
    }

    public class WeekInOutCountDTO
    {
        /// <summary>
        /// 날짜구분
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 입고 카운트
        /// </summary>
        public int InputCount { get; set; }

        /// <summary>
        /// 출고 카운트
        /// </summary>
        public int OutputCount { get; set; }

        /// <summary>
        /// 입-출고 카운트
        /// </summary>
        public int TotalCount { get; set; }
    }
}
