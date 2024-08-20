namespace FamTec.Shared.Server.DTO.Material
{
    /// <summary>
    /// 엑셀 Import DATA
    /// </summary>
    public class ExcelMaterialInfo
    {
        /// <summary>
        /// 자재코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 자재명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? MFC { get; set; }

        /// <summary>
        /// 안전재고 수량
        /// </summary>
        public string? SafeNum { get; set; }

        /// <summary>
        /// 위치
        /// </summary>
        public string? Location { get; set; }
    }
}
