namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class MaterialCountDTO
    {
        /// <summary>
        /// 자재 ID
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// 자재명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 자재총합
        /// </summary>
        public int TotalNum { get; set; }

        /// <summary>
        /// 안전재고수량
        /// </summary>
        public int SafeNum { get; set; }

        /// <summary>
        /// 차이
        /// </summary>
        public int Distance { get; set; }
    }
}
