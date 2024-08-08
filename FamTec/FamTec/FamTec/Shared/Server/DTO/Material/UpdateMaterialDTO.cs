namespace FamTec.Shared.Server.DTO.Material
{
    public class UpdateMaterialDTO
    {
        /// <summary>
        /// 품목ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 품목 코드
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string Unit { get; set; }

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
        /// 공간ID
        /// </summary>
        public int RoomID { get; set; }
    }
}
