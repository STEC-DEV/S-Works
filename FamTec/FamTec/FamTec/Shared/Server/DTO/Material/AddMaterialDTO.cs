namespace FamTec.Shared.Server.DTO.Material
{
    public class AddMaterialDTO
    {
        /// <summary>
        /// 품목 코드
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
        public string? ManufacturingComp { get; set; }

        /// <summary>
        /// 안전재고 수량
        /// </summary>
        public int? SafeNum { get; set; }

        /// <summary>
        /// 자재위치
        /// </summary>
        public int? DefaultLocation { get; set; }

    }
}
