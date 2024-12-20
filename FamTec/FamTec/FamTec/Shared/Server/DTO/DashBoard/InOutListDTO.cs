namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class InOutListDTO
    {
        /// <summary>
        /// 입고 List
        /// </summary>
        public List<InOutDataDTO> InPutList { get; set; } = new List<InOutDataDTO>();

        /// <summary>
        /// 출고 List
        /// </summary>
        public List<InOutDataDTO> OutPutList { get; set; } = new List<InOutDataDTO>();

        /// <summary>
        /// 입출고 총카운트
        /// </summary>
        public int InOutCount { get; set; }
    }

    public class InOutDataDTO
    {
        /// <summary>
        /// StoreID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }

        /// <summary>
        /// 입출고가격
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 공간ID
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 자재ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 자재명칭
        /// </summary>
        public string? MaterialName { get; set; }

    }
}
