namespace FamTec.Shared.Server.DTO.Store
{
    public class InOutLocationDTO
    {
        /// <summary>
        /// 공간ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 공간 명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 자재ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 해당 공간의 품목수량
        /// </summary>
        public int Num { get; set; }
    }
}
