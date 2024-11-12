namespace FamTec.Shared.Server.DTO.Excel
{
    public class ExcelFacilityInfo
    {
        /// <summary>
        /// 설비명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 설비형식
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 공간인덱스
        /// </summary>
        public int? RoomId { get; set; }

        /// <summary>
        /// 규격용량
        /// </summary>
        public string? StandardCapacity { get; set; }
        
        /// <summary>
        /// 수량
        /// </summary>
        public int? Num { get; set; }
        
        /// <summary>
        /// 내용년수
        /// </summary>
        public string? LifeSpan { get; set; }

        /// <summary>
        /// 설치년월
        /// </summary>
        public DateTime? EquipDT { get; set; }

        /// <summary>
        /// 교체년월
        /// </summary>
        public DateTime? ChangeDT { get; set; }


    }
}
