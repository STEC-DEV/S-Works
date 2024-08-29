namespace FamTec.Shared.Server.DTO.Material
{
    public class MaterialSearchListDTO
    {
        /// <summary>
        /// 품목ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 품목 코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? Mfr { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }
    }
}
