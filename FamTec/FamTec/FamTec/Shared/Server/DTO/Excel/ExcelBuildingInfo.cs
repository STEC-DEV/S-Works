namespace FamTec.Shared.Server.DTO.Excel
{
    public class ExcelBuildingInfo
    {
        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }
        
        /// <summary>
        /// 건물주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 대표전화
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public DateTime? CompletionDT { get; set; }

        /// <summary>
        /// 건물구조
        /// </summary>
        public string? BuildingStruct { get; set; }

        /// <summary>
        /// 건물용도
        /// </summary>
        public string? Usage { get; set; }

        /// <summary>
        /// 시공업체
        /// </summary>
        public string? ConstComp { get; set; }

        /// <summary>
        /// 지붕구조
        /// </summary>
        public string? RoofStruct { get; set; }

        /// <summary>
        /// 소방등급
        /// </summary>
        public string? FireRatingNum { get; set; }

        /// <summary>
        /// 정화조용량
        /// </summary>
        public string? SepticTankCapacity { get; set; }

    }
}
